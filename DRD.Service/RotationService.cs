using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class RotationService
    {
        //helper
        private bool CheckIdExist(long id)
        {
            using var db = new Connection();
            return db.Rotations.Any(i => i.Id == id);
        }

        public ResponseStatus Start(long userId, long rotationId, long subscriptionId)
        {
            var returnItem = StartProcess(userId, rotationId, subscriptionId);
            InboxService inboxService = new InboxService();
            List<int> returnValue = new List<int>();
            if (returnItem != null)
            {
                foreach (ActivityItem act in returnItem)
                {
                    returnValue.Add(inboxService.CreateInbox(act));
                }
                return new ResponseStatus() { code = returnItem[0].ExitCode, status = returnItem[0].ExitStatus };
            }
            return new ResponseStatus() { code = -6, status = "" };
        }

        //helper
        private List<ActivityItem> StartProcess(long userId, long rotationId, long usageId)
        {
            List<ActivityItem> result = new List<ActivityItem>();
            SubscriptionService subscriptionService = new SubscriptionService();

            //Check Subscription
            Constant.BusinessUsageStatus subscriptionStatus = subscriptionService.IsSubscriptionValid(userId, usageId);
            if (subscriptionStatus.Equals(Constant.BusinessUsageStatus.EXTENDED))
            {
                usageId = subscriptionService.GetActiveUsageFromInactiveUsage(usageId);
                subscriptionStatus = Constant.BusinessUsageStatus.OK;
            }

            if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
            {
                result.Add(CreateActivityResult((int)subscriptionStatus, exitStatus: subscriptionStatus.ToString()));
                return result;
            }

            //check rotation status before start
            using var db = new Connection();
            var rotationDb = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            if (!rotationDb.Status.Equals((int)Constant.RotationStatus.Open))
            {
                result.Add(CreateActivityResult((int)Constant.RotationStatus.ERROR_ROTATION_ALREADY_STARTED, exitStatus: Constant.RotationStatus.ERROR_ROTATION_ALREADY_STARTED.ToString()));
                return result; //Invalid rotation
            }

            //update rotation
            var companyUsage = db.BusinessUsages.FirstOrDefault(c => c.Id == usageId && c.IsActive);
            rotationDb.Status = (int)Constant.RotationStatus.In_Progress;
            rotationDb.CompanyId = companyUsage.CompanyId;
            rotationDb.SubscriptionType = companyUsage.CompanyId != 0 ? (byte)Models.Constant.SubscriptionType.BUSINESS : (byte)Models.Constant.SubscriptionType.PERSONAL;

            // first node, node after start symbol
            var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.Source.WorkflowId == rotationDb.WorkflowId && c.Source.SymbolCode == 0).ToList();
            if (workflowNodeLinks == null)
            {
                result.Add(CreateActivityResult((int)Constant.RotationStatus.ERROR_WORKFLOW_START_NODE_NOT_FOUND, exitStatus: Constant.RotationStatus.ERROR_WORKFLOW_START_NODE_NOT_FOUND.ToString()));
                return result; //Invalid rotation
            }

            //check rotation started limit or add when limit passed
            var rotationStartedLimitStatus = subscriptionService.CheckOrAddSpecificUsage(Models.Constant.BusinessPackageItem.Rotation_Started, rotationDb.CompanyId.Value, additional: 1, addAfterSubscriptionValid: true);

            if (!rotationStartedLimitStatus.Equals(Constant.BusinessUsageStatus.OK))
            {
                result.Add(CreateActivityResult((int)rotationStartedLimitStatus, exitStatus: rotationStartedLimitStatus.ToString()));
                return result;
            }

            // send to all activity under start node +
            foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
            {
                long userNodeId = GetUserId(workflowNodeLink.TargetId, rotationDb.Id);
                RotationNode rtnode = new RotationNode
                {
                    RotationId = rotationDb.Id,
                    WorkflowNodeId = workflowNodeLink.TargetId,
                    WorkflowNode = workflowNodeLink.Target,
                    FirstNodeId = workflowNodeLink.FirstNodeId,

                    UserId = userNodeId,
                    Status = (int)Constant.RotationStatus.Open,
                    CreatedAt = DateTime.Now
                };
                db.RotationNodes.Add(rtnode);
                db.SaveChanges();
                result.Add(CreateActivityResult(1, rtnode.UserId, userId, rotationDb.Name, rtnode.Id, rotationId, exitStatus: Constant.RotationStatus.OK.ToString()));
            }

            db.SaveChanges();
            return result;
        }

        //helper
        private ActivityItem CreateActivityResult(int exitCode, long userId = 0, long previousUserId = 0, string rotationName = "", long rotationNodeId = 0, long rotationId = 0, string exitStatus = null)
        {
            using var db = new Connection();
            var currentUserDb = db.Users.FirstOrDefault(c => c.Id == userId);
            var previousUserDb = db.Users.FirstOrDefault(c => c.Id == previousUserId);
            ActivityItem result = new ActivityItem
            {
                RotationId = rotationId,
                UserId = userId,
                UserName = currentUserDb.Name,
                Email = currentUserDb.Email,

                PreviousUserId = previousUserId,
                PreviousUserName = previousUserDb.Name,
                PreviousEmail = previousUserDb.Email,

                RotationName = rotationName,
                RotationNodeId = rotationNodeId,

                ExitCode = exitCode,
                ExitStatus = exitStatus
            };
            result.ExitStatus ??= Constant.RotationStatus.OK.ToString();
            return result;
        }

        //helper
        private long GetUserId(long WorkflowNodeToId, long RotationId)
        {
            using var db = new Connection();
            return db.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == WorkflowNodeToId && c.RotationId == RotationId).UserId.Value;
        }

        /// <summary>
        /// Obtain rotation that user has already made and search by Id of the rotation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        public RotationIndex GetRotation(long id, long creatorId, bool isActive = true)
        {
            using var db = new Connection();
            var result =
                        (from rotation in db.Rotations
                         where rotation.Id == id && rotation.IsActive == isActive && rotation.CreatorId == creatorId
                         select new RotationIndex
                         {
                             Id = rotation.Id,
                             Name = rotation.Name,
                             Description = rotation.Description,
                             WorkflowId = rotation.WorkflowId,
                             CompanyId = rotation.CompanyId,
                             Workflow = new WorkflowItem
                             {
                                 Id = rotation.Workflow.Id,
                                 Name = rotation.Workflow.Name,
                             },
                             Status = rotation.Status,
                             UserId = rotation.UserId, // filled when using personal plan
                             RotationUsers = (from x in rotation.RotationUsers
                                              select new RotationUserItem
                                              {
                                                  Id = x.Id,
                                                  UserId = x.UserId,
                                                  WorkflowNodeId = x.WorkflowNodeId,
                                                  ActivityName = x.WorkflowNode.Caption,
                                                  Number = (x.UserId == null ? (long?)null : x.User.Id),
                                                  Name = (x.UserId == null ? "Undefined" : x.User.Name),
                                                  Email = (x.UserId == null ? "" : x.User.Email),
                                                  Picture = (x.UserId == null ? "icon_user.png" : x.User.ProfileImageFileName),
                                                  FlagPermission = x.ActionPermission
                                              }).ToList(),
                         }).FirstOrDefault();
            //check if result null
            if (result == null) return result;

            //add encrypted id to access image folder
            foreach (RotationUserItem x in result.RotationUsers)
                x.EncryptedId = Utilities.Encrypt(x.UserId.ToString());

            result.CompanyRotation = (from cmpny in db.Companies
                                      where cmpny.Id == result.CompanyId
                                      select new SmallCompanyData
                                      {
                                          Id = cmpny.Id,
                                          Code = cmpny.Code,
                                          Name = cmpny.Name,
                                      }).FirstOrDefault();
            var tagService = new TagService();
            result.Tags = tagService.GetTagsAsString(result.Id);
            return result;
        }

        /// <summary>
        /// Obtain activities/Workflow Node from Workflow
        /// </summary>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        public IEnumerable<RotationUserData> GetWorkflowActivities(long workflowId)
        {
            using var db = new Connection();
            var result =
                (from workflowNode in db.WorkflowNodes
                 where workflowNode.WorkflowId == workflowId && workflowNode.SymbolCode == 5
                 select new RotationUserData
                 {
                     ActivityName = workflowNode.Caption,
                     WorkflowNodeId = workflowNode.Id
                 }).ToList();
            foreach (RotationUserData item in result)
            {
                if (item.UserId != 0)
                {
                    User user = (from Userdb in db.Users where Userdb.Id == item.UserId select Userdb).FirstOrDefault();
                    if (user != null)
                    {
                        item.Email = user.Email;
                        item.Name = user.Name;
                        item.Picture = user.ProfileImageFileName;
                    }
                }
            }
            result = result.OrderBy(c => c.WorkflowNodeId).ToList();
            return result;
        }

        /// <summary>
        /// Obtain Rotation related to the criteria, and user as the creator. Take as many as given parameters
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="criteria"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public ICollection<RotationListItem> GetRotations(long creatorId, string criteria, int skip, int take, bool isActive = true)
        {
            // top criteria
            string[] criterias = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
            {
                criterias = criteria.Split(' ');
            }
            else
                criteria = "";
            using var db = new Connection();
            if (db.Rotations == null) return null;
            var result = (from rotation in db.Rotations
                          where rotation.CreatorId == creatorId && rotation.IsActive == isActive && (criteria.Equals("") || criterias.All(criteria => (rotation.Name).ToLower().Contains(criteria.ToLower())))
                          orderby rotation.Status, rotation.CreatedAt descending, rotation.Name descending
                          select new RotationListItem
                          {
                              Id = rotation.Id,
                              Name = rotation.Name,
                              Status = rotation.Status,
                              WorkflowId = rotation.Workflow.Id,
                              WorkflowName = rotation.Workflow.Name,
                              CompanyId = rotation.CompanyId,
                              CreatedAt = rotation.CreatedAt,
                              UpdatedAt = rotation.UpdatedAt,
                              StartedAt = rotation.UpdatedAt,
                          }).Skip(skip).Take(take).ToList();
            foreach (RotationListItem rotationListItem in result)
            {
                rotationListItem.CompanyRotation = (from cmpny in db.Companies
                                                    where cmpny.Id == rotationListItem.CompanyId
                                                    select new SmallCompanyData
                                                    {
                                                        Id = cmpny.Id,
                                                        Code = cmpny.Code,
                                                        Name = cmpny.Name,
                                                    }).FirstOrDefault();
                rotationListItem.StatusDescription = rotationListItem.Status == (int)Constant.RotationStatus.Open ? "Not Started" : (rotationListItem.Status == (int)Constant.RotationStatus.In_Progress ? "Ongoing" : "Completed");
            }
            return result;
        }

        /// <summary>
        /// Count Rotation related to the criteria, and user as the creator.
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public int CountRotations(long creatorId, string criteria, bool isActive = true)
        {
            // top criteria
            string[] criterias = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
                criterias = criteria.Split(' ');
            else
                criteria = "";

            using (var db = new Connection())
            {
                if (db.Rotations != null)
                {
                    var result = (from rotation in db.Rotations
                                  where rotation.CreatorId == creatorId && rotation.IsActive == isActive && (criteria.Equals("") || criterias.All(criteria => (rotation.Name).ToLower().Contains(criteria.ToLower())))
                                  orderby rotation.Status, rotation.CreatedAt descending, rotation.Name descending
                                  select new RotationData
                                  {
                                      Id = rotation.Id,
                                  }).Count();
                    return result;
                }
                return 0;
            }
        }

        /// <summary>
        /// Save new or updated rotation
        /// </summary>
        /// <param name="newRotation"></param>
        /// <returns></returns>
        public long Save(RotationItem newRotation)
        {
            Rotation rotationDb;
            using var db = new Connection();
            if (newRotation.Id != 0)
            {
                rotationDb = db.Rotations.FirstOrDefault(c => c.Id == newRotation.Id);
            }
            else
            {
                rotationDb = new Rotation();
                while (CheckIdExist(rotationDb.Id))
                {
                    rotationDb.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                }
            }

            Workflow workflowDb = db.Workflows.FirstOrDefault(w => w.Id == newRotation.WorkflowId);
            rotationDb.Name = newRotation.Name;
            rotationDb.Workflow = workflowDb;
            rotationDb.Description = newRotation.Description;
            rotationDb.Status = newRotation.Status;
            rotationDb.CreatorId = newRotation.CreatorId;
            rotationDb.UserId = newRotation.UserId;

            if (newRotation.Id == 0)
            {
                db.Rotations.Add(rotationDb);
                workflowDb.TotalUsed += 1;
            }
            var result = db.SaveChanges();
            // prepare data detail
            var cxold = db.RotationUsers.Count(c => c.Rotation.Id == rotationDb.Id);
            var cxnew = newRotation.RotationUsers.Count();
            if (cxold < cxnew)
            {
                bool startPersonAded = false;
                // save detail
                for (var x = 0; x < cxnew; x++)
                {
                    var ep = newRotation.RotationUsers.ElementAt(x); // get the data
                    var newItem = new RotationUser
                    {
                        Rotation = rotationDb,
                        WorkflowNodeId = ep.Id
                    };
                    var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == ep.WorkflowNodeId);
                    if (!startPersonAded)
                    {
                        var checkIsStartNode = (from workflowNode in db.WorkflowNodes
                                                join wfndLink in db.WorkflowNodeLinks on workflowNode.Id equals wfndLink.SourceId
                                                where workflowNode.WorkflowId == wfl.WorkflowId
                                                && workflowNode.SymbolCode == 0
                                                && wfndLink.TargetId == ep.WorkflowNodeId //this node is a target node from start Node
                                                select new
                                                {
                                                    isFound = true
                                                }).ToList();
                        // if this RotationUser is startNode.
                        if (checkIsStartNode.Count == 1)
                        {
                            newItem.IsStartPerson = true;
                            // only one person can be startNode
                            startPersonAded = true;
                        }
                    }
                    newItem.WorkflowNode = wfl;
                    newItem.ActionPermission = ep.ActionPermission;
                    User gotUser = db.Users.FirstOrDefault(usr => usr.Id == ep.UserId);
                    newItem.User = gotUser;
                    newItem.UserId = gotUser.Id;
                    rotationDb.RotationUsers.Add(newItem);
                    db.RotationUsers.Add(newItem);
                    db.SaveChanges();
                }
            }
            else if (cxold > cxnew)
            {
                var dremove = db.RotationUsers.Where(c => c.Rotation.Id == rotationDb.Id).Take(cxold - cxnew).ToList();
                db.RotationUsers.RemoveRange(dremove);
                db.SaveChanges();
                // save detail
                var dnew = db.RotationUsers.Where(c => c.Rotation.Id == rotationDb.Id).ToList();
                int v = 0;
                foreach (RotationUser d in dnew)
                {
                    var epos = newRotation.RotationUsers.ElementAt(v);
                    var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == epos.WorkflowNodeId);
                    d.WorkflowNodeId = wfl.Id;
                    d.WorkflowNode = wfl;
                    wfl.RotationUsers.Add(d);
                    User gotUser = db.Users.FirstOrDefault(usr => usr.Id == epos.UserId);
                    d.User = gotUser;
                    d.UserId = gotUser.Id;
                    d.ActionPermission = epos.ActionPermission;
                    v++;
                    db.SaveChanges();
                }
            }
            // kalau jumlah sama harus di cek setiap elemennya apakah tidak berubah
            else
            {
                var rotationUserOld = db.RotationUsers.Where(c => c.Rotation.Id == rotationDb.Id).ToList();
                int v = 0;
                foreach (RotationUser userItem in rotationUserOld)
                {
                    var newRotationUser = newRotation.RotationUsers.ElementAt(v);
                    var workflowNodeDb = db.WorkflowNodes.FirstOrDefault(c => c.Id == newRotationUser.WorkflowNodeId);
                    userItem.WorkflowNodeId = workflowNodeDb.Id;
                    userItem.WorkflowNode = workflowNodeDb;
                    workflowNodeDb.RotationUsers.Add(userItem);
                    User gotUser = db.Users.FirstOrDefault(usr => usr.Id == newRotationUser.UserId);
                    userItem.User = gotUser;
                    userItem.UserId = gotUser.Id;
                    userItem.ActionPermission = newRotationUser.ActionPermission;
                    v++;
                    db.SaveChanges();
                }
            }
            // Tags this implementation is for rotation save or update(there is rotation in db that editted)
            var tagItemListInDb = db.TagItems.Where(tagitemdb => tagitemdb.RotationId == rotationDb.Id).ToList();
            if (newRotation.Tags != null)
                foreach (string tag in newRotation.Tags)
                {
                    var tagfromDB = db.Tags.FirstOrDefault(tagdb => tagdb.Name.Equals(tag));
                    if (tagfromDB == null)
                    {
                        tagfromDB = new Tag();
                        tagfromDB.Name = tag;
                        db.Tags.Add(tagfromDB);
                        db.SaveChanges();
                    }
                    var tagItemFromDb = tagItemListInDb.FirstOrDefault(tagitemdb => tagitemdb.TagId == tagfromDB.Id && tagitemdb.RotationId == rotationDb.Id);
                    if (tagItemFromDb == null)
                    {
                        tagItemFromDb = new TagItem();
                        tagItemFromDb.RotationId = rotationDb.Id;
                        tagItemFromDb.Rotation = rotationDb;
                        tagItemFromDb.TagId = tagfromDB.Id;
                        tagItemFromDb.Tag = tagfromDB;
                        db.TagItems.Add(tagItemFromDb);
                        rotationDb.TagItems.Add(tagItemFromDb);
                    }
                    else
                    {
                        tagItemListInDb.Remove(tagItemFromDb);
                    }
                }
            if (tagItemListInDb.Count() != 0)
            {
                foreach (var item in tagItemListInDb)
                {
                    db.TagItems.Remove(item);
                }
            }
            db.SaveChanges();

            return rotationDb.Id;
        }

        /// <summary>
        /// Delete(inactive) specific rotation from id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Delete(long id)
        {
            using var db = new Connection();
            var rotationDb = db.Rotations.Where(i => i.Id == id).FirstOrDefault();

            //check if rotation exist
            if (rotationDb == null) return Constant.RotationStatus.NOT_FOUND.ToString();

            //check is already being used or not
            if (rotationDb.CompanyId.HasValue) return Constant.RotationStatus.ERROR_ROTATION_ALREADY_STARTED.ToString();

            rotationDb.IsActive = false;
            rotationDb.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return Constant.RotationStatus.OK.ToString();
        }
    }
}