using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DRD.Service
{
    public class RotationService
    {
        private readonly string _connString;
        private string _appZoneAccess;
        private Constant constant = new Constant();

        public RotationService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }

        public RotationService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = Constant.CONSTRING;
        }

        public RotationService()
        {
            _connString = Constant.CONSTRING;
        }

       

        public RotationInboxData AssignNodes(ServiceContext db, RotationInboxData rot, long userId, IDocumentService docSvr)
        {
            RotationInboxData rotation = rot;

            rotation.RotationNodes =
                (from rotationNode in db.RotationNodes
                 where rotationNode.Rotation.Id == rotation.Id
                 orderby rotationNode.CreatedAt
                 select new RotationNodeInboxData
                 {
                     Id = rotationNode.Id,
                     CreatedAt = rotationNode.CreatedAt,
                     Status = rotationNode.Status,
                     WorkflowNodeId = rotationNode.WorkflowNodeId,
                     PrevWorkflowNodeId = rotationNode.PreviousWorkflowNodeId,
                     SenderRotationNodeId = rotationNode.SenderRotationNodeId,
                     User = new UserInboxData
                     {
                         Id = rotationNode.User.Id,
                         Name = rotationNode.User.Name,
                         ImageProfile = rotationNode.User.ProfileImageFileName,
                         ImageInitials = rotationNode.User.InitialImageFileName,
                         ImageSignature = rotationNode.User.SignatureImageFileName,
                         ImageStamp = rotationNode.User.StampImageFileName,
                         ImageKtp1 = rotationNode.User.KTPImageFileName,
                         ImageKtp2 = rotationNode.User.KTPVerificationImageFileName,
                     },
                     WorkflowNode = new WorkflowNodeInboxData
                     {
                         Id = rotationNode.WorkflowNode.Id,
                         Caption = rotationNode.WorkflowNode.Caption
                     },
                 }).ToList();

            //if owner has access to readonly
            rotation.AccessType = rotation.CreatorId == userId ? (int)Constant.AccessType.readOnly : (int)Constant.AccessType.noAccess;

            foreach (RotationNodeInboxData rotationNode in rotation.RotationNodes)
            {
                //set page access to specific user
                if (rotationNode.User.Id == userId)
                {
                    rotation.AccessType = (int)Constant.AccessType.readOnly;
                    // responsible access for the current user
                    if (rotationNode.Status.Equals((int)Constant.RotationStatus.Open))
                    {
                        rotation.AccessType = (int)Constant.AccessType.responsible;
                    }
                }

                // user encrypted id
                rotationNode.User.EncryptedUserId = Utilities.Encrypt(rotationNode.User.Id.ToString());

                rotationNode.RotationNodeDocs = AssignNodeDocs(db, rotationNode.Id, userId, rot.RotationNodeId, docSvr);
               
                //document summaries document
                foreach (RotationNodeDocInboxData rotationNodeDoc in rotationNode.RotationNodeDocs)
                {
                    // get anno
                    foreach (DocumentElementInboxData documentElement in rotationNodeDoc.Document.DocumentElements)
                    {
                        if (documentElement.ElementId == null || documentElement.ElementId == 0) continue;
                        if (documentElement.ElementTypeId == (int)Constant.EnumElementTypeId.SIGNATURE 
                            || documentElement.ElementTypeId == (int)Constant.EnumElementTypeId.INITIAL 
                            || documentElement.ElementTypeId == (int)Constant.EnumElementTypeId.PRIVATESTAMP)
                        {
                            var user = db.Users.FirstOrDefault(c => c.Id == documentElement.ElementId);
                            Element newElement = new Element();
                            newElement.EncryptedUserId = Utilities.Encrypt(user.Id.ToString());
                            newElement.UserId = user.Id;
                            newElement.Name = user.Name;
                            newElement.Foto = user.ProfileImageFileName;
                            documentElement.Element = newElement;
                        }
                       
                    }

                    var dx = rotation.SumRotationNodeDocs.FirstOrDefault(c => c.Document.Id == rotationNodeDoc.Document.Id);
                    if (dx != null)
                    {
                        dx.FlagAction |= rotationNodeDoc.FlagAction;
                    }
                    else
                    {
                        rotation.SumRotationNodeDocs.Add(DeepCopy(rotationNodeDoc));
                    }
                }
                
            }
            return rotation;
        }
        /// <summary>
        /// Count Rotation related to the criteria, and user as the creator.
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="topCriteria"></param>
        /// <returns></returns>
        public int CountRotations(long creatorId, string topCriteria, bool isActive = true)
        {
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                if (db.Rotations != null)
                {
                    var result = (from rotation in db.Rotations
                                  where rotation.CreatorId == creatorId && rotation.IsActive==isActive && (topCriteria.Equals("") || tops.All(criteria => (rotation.Name).ToLower().Contains(criteria.ToLower())))
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
        /// Obtain Rotation related to the criteria, and user as the creator. Take as many as given parameters
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public ICollection<RotationData> GetRotations(long creatorId, string topCriteria, int skip, int take, bool isActive = true)
        {
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
            {
                tops = topCriteria.Split(' ');
            }
            else
                topCriteria = "";
            using (var db = new ServiceContext())
            {
                if (db.Rotations == null) return null;
                var result = (from rotation in db.Rotations
                              where rotation.CreatorId == creatorId && rotation.IsActive == isActive && (topCriteria.Equals("") || tops.All(criteria => (rotation.Name).ToLower().Contains(criteria.ToLower())))
                              orderby rotation.Status, rotation.CreatedAt descending, rotation.Name descending
                              select new RotationData
                              {
                                  Id = rotation.Id,
                                  Subject = rotation.Name,
                                  Status = rotation.Status,
                                  WorkflowId = rotation.Workflow.Id,
                                  WorkflowName = rotation.Workflow.Name,
                                  CompanyId = rotation.CompanyId,
                                  UserId = rotation.UserId,
                                  CreatedAt = rotation.CreatedAt,
                                  UpdatedAt = rotation.UpdatedAt,
                                  StartedAt = rotation.UpdatedAt,
                              }).Skip(skip).Take(take).ToList();
                foreach (RotationData resultItem in result)
                {
                    resultItem.CompanyRotation = (from cmpny in db.Companies
                                                  where cmpny.Id == resultItem.CompanyId
                                                  select new SmallCompanyData
                                                  {
                                                      Id = cmpny.Id,
                                                      Code = cmpny.Code,
                                                      Name = cmpny.Name,
                                                  }).FirstOrDefault();
                }
                return result;
            }
        }

        public Rotation GetById(long id, long userId = 0)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.Id == id
                     select new Rotation
                     {
                         Id = rotation.Id,
                         Name = rotation.Name,
                         Status = rotation.Status,
                         Description = rotation.Description,
                         UserId = rotation.UserId,
                         CreatedAt = rotation.CreatedAt,
                         UpdatedAt = rotation.UpdatedAt,
                         StartedAt = rotation.StartedAt,
                         Workflow = new Workflow
                         {
                             Id = rotation.Workflow.Id,
                             Name = rotation.Workflow.Name,
                         }
                     }).FirstOrDefault();
                //result = assignNodes(db, result, userId, new DocumentService());
                return result;
            }
        }

        public IEnumerable<Rotation> GetByUserId(long userId)
        {
            using (var db = new ServiceContext())
            {
                int[] finishedStatus = { (int)Constant.RotationStatus.Completed, (int)Constant.RotationStatus.Canceled, (int)Constant.RotationStatus.Declined };
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && !finishedStatus.Contains(rotation.Rotation.Status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.UserId == userId) && !finishedStatus.Contains(rotation.Status)).ToList();

                List<Rotation> rotations = new List<Rotation>();
                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.UpdatedAt).ToList();
                return rotations;
            }
        }

        public IEnumerable<Rotation> GetByUserId(long userId, string status)
        {
            using (var db = new ServiceContext())
            {
                var statuses = status.Split(',').Select(Int32.Parse).ToList();

                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && statuses.Contains(rotation.Rotation.Status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();

                var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.UserId == userId) && statuses.Contains(rotation.Status)).ToList();

                List<Rotation> rotations = new List<Rotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.UpdatedAt).ToList();
                return rotations;
            }
        }

        /// <summary>
        /// Obtain rotation that user has already made and search by Id of the rotation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        public RotationIndex GetRotationById(long id, long creatorId, bool isActive = true)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.Id == id && rotation.IsActive==isActive && rotation.CreatorId == creatorId
                     select new RotationIndex
                     {
                         Id = rotation.Id,
                         Subject = rotation.Name,
                         Remark = rotation.Description,
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
                                              FlagPermission = x.ActionPermission,
                                              //FlagAction = x.FlagAction,
                                              //CxDownload = x.CxDownload,
                                              //CxPrint = x.CxPrint,
                                          }).ToList(),
                     }).FirstOrDefault();
                foreach (RotationUserItem x in result.RotationUsers)
                {
                    x.EncryptedId = Utilities.Encrypt(x.UserId.ToString());
                }
                if (result == null) return result;
                result.CompanyRotation = (from cmpny in db.Companies
                                          where cmpny.Id == result.CompanyId
                                          select new SmallCompanyData
                                          {
                                              Id = cmpny.Id,
                                              Code = cmpny.Code,
                                              Name = cmpny.Name,
                                          }).FirstOrDefault();
                var tagService = new TagService();
                var tags = tagService.GetTags(result.Id);
                result.Tags = (from tag in tags select tag.Name).ToList();
                return result;
            }
        }
        public IEnumerable<RotationUserData> GetUsersWorkflow(long workflowId)
        {
            using var db = new ServiceContext();
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
        ///
        /// </summary>
        /// <parameter name="prod"></parameter>
        /// <returns></returns>
        public long Save(RotationItem prod)
        {
            WorkflowDeepService workflowDeepService = new WorkflowDeepService();
            return workflowDeepService.Save(prod);
        }
        private static RotationNodeDocInboxData DeepCopy(RotationNodeDocInboxData source)
        {
            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<RotationNodeDocInboxData>(JsonConvert.SerializeObject(source), DeserializeSettings);
        }
        /// <summary>
        /// Obtain all the Rotation Node Document will return as inbox data
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rnId"></param>
        /// <param name="usrId"></param>
        /// <param name="curRnId"></param>
        /// <param name="docSvr"></param>
        /// <returns></returns>
        private List<RotationNodeDocInboxData> AssignNodeDocs(ServiceContext db, long rnId, long usrId, long? curRnId, IDocumentService docSvr)
        {
            if (curRnId == 0)
                curRnId = -rnId;
            var rndFromDb = db.RotationNodeDocs.Where(rnd => rnd.RotationNodeId == rnId).ToList();
            List<RotationNodeDocInboxData> result = new List<RotationNodeDocInboxData>();
            foreach (var rndDb in rndFromDb)
            {
                var item = new RotationNodeDocInboxData();
                item.Id = rndDb.Id;
                item.FlagAction = rndDb.ActionStatus;
                item.DocumentId = rndDb.DocumentId;
                item.RotationNode.RotationId = rndDb.RotationId;
                item.Document.Id = rndDb.Document.Id;
                item.Document.Extention = rndDb.Document.Extention;
                item.Document.FileUrl = rndDb.Document.FileUrl;
                item.Document.FileName = rndDb.Document.FileName;
                item.Document.FileSize = rndDb.Document.FileSize;
                item.Document.IsCurrent = rndDb.Document.IsCurrent;
                item.Document.CreatedAt = rndDb.Document.CreatedAt;
                item.Document.UpdatedAt = rndDb.Document.CreatedAt;
                foreach (var dusr in rndDb.Document.DocumentUsers)
                {
                    var dUsrItem = new DocumentUserInboxData();
                    dUsrItem.Id = dusr.Id;
                    dUsrItem.DocumentId = dusr.DocumentId;
                    dUsrItem.UserId = dusr.UserId;
                    dUsrItem.FlagAction = dusr.ActionStatus;
                    dUsrItem.FlagPermission = dusr.ActionPermission;
                    dUsrItem.FlagPermission |= docSvr.GetPermission(usrId, curRnId.Value, dusr.DocumentId);
                    item.Document.DocumentUsers.Add(dUsrItem);
                }
                foreach (var delm in rndDb.Document.DocumentElements) 
                {
                    var dElmItem = new DocumentElementInboxData();
                    dElmItem.Id = delm.Id;
                    dElmItem.DocumentId = delm.DocumentId;
                    dElmItem.Page = delm.Page;
                    dElmItem.LeftPosition = delm.LeftPosition;
                    dElmItem.TopPosition = delm.TopPosition;
                    dElmItem.WidthPosition = delm.WidthPosition;
                    dElmItem.HeightPosition = delm.HeightPosition;
                    dElmItem.Color = delm.Color;
                    dElmItem.BackColor = delm.BackColor;
                    dElmItem.Data = delm.Text;
                    dElmItem.Data2 = delm.Unknown;
                    dElmItem.Rotation = delm.Rotation;
                    dElmItem.ScaleX = delm.ScaleX;
                    dElmItem.ScaleY = delm.ScaleY;
                    dElmItem.TransitionX = delm.TransitionX;
                    dElmItem.TransitionY = delm.TransitionY;
                    dElmItem.StrokeWidth = delm.StrokeWidth;
                    dElmItem.Opacity = delm.Opacity;
                    dElmItem.Flag = delm.Flag;
                    dElmItem.FlagCode = delm.AssignedAnnotationCode;
                    dElmItem.FlagDate = delm.AssignedAt;
                    dElmItem.FlagImage = delm.AssignedAnnotationImageFileName;
                    dElmItem.CreatorId = delm.CreatorId;
                    dElmItem.ElementId = delm.ElementId;
                    dElmItem.UserId = delm.UserId;
                    dElmItem.CreatedAt = delm.CreatedAt;
                    dElmItem.UpdatedAt = delm.UpdatedAt;
                    dElmItem.ElementTypeId = delm.ElementTypeId;
                    item.Document.DocumentElements.Add(dElmItem);
                }
                item.Document.DocumentUser = item.Document.DocumentUsers.FirstOrDefault(itmDocUsr => itmDocUsr.UserId == usrId);
                result.Add(item);
            }
            return result;
        }

        public ICollection<RotationDashboard> GetRelatedToCompany(long companyId, ICollection<string> tags, int skip, int pageSize)
        {
            using (var db = new ServiceContext())
            {
                var data = (from rotation in db.Rotations
                            where rotation.CompanyId == companyId
                            orderby rotation.UpdatedAt descending
                            select new RotationDashboard
                            {
                                Id = rotation.Id,
                                Subject = rotation.Name,
                                Status = rotation.Status,
                                DateCreated = rotation.CreatedAt,
                                DateUpdated = rotation.UpdatedAt,
                                DateStarted = rotation.StartedAt,
                                Tags = (from tagitem in rotation.TagItems
                                        join tag in db.Tags on tagitem.TagId equals tag.Id
                                        select tag.Name.ToLower()).ToList(),
                                RotationUsers = (from rtuser in rotation.RotationUsers
                                                 select new RotationDashboard.UserDashboard
                                                 {
                                                     Id = rtuser.User.Id,
                                                     Name = rtuser.User.Name,
                                                     ImageProfile = rtuser.User.ProfileImageFileName
                                                 }).ToList(),
                                Creator = (from user in db.Users
                                           where user.Id == rotation.CreatorId
                                           select new RotationDashboard.UserDashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               ImageProfile = user.ProfileImageFileName
                                           }).FirstOrDefault(),
                                Workflow = new RotationDashboard.WorkflowDashboard
                                {
                                    Id = rotation.Workflow.Id,
                                    Name = rotation.Workflow.Name
                                }
                            });
                if (tags != null)
                    data = data.Where(item => tags.All(itag => item.Tags.Contains(itag.ToLower())));
                if (pageSize > 0 && skip >= 0)
                    data = data.Skip(skip).Take(pageSize);
                var result = data.ToList();
                foreach (RotationDashboard x in result)
                {
                    x.InboxId = db.Inboxes.Where(inbox => inbox.RotationId == x.Id).FirstOrDefault().Id;
                    x.Creator.EncryptedId = Utilities.Encrypt(x.Creator.Id.ToString());
                    foreach (RotationDashboard.UserDashboard y in x.RotationUsers)
                    {
                        var rNode = (from rotationNode in db.RotationNodes
                                     where rotationNode.Rotation.Id == x.Id
                                     && rotationNode.UserId == y.Id
                                     select new RotationNodeInboxData
                                     {
                                         CreatedAt = rotationNode.CreatedAt,
                                         Status = rotationNode.Status
                                     }).FirstOrDefault();
                        if (rNode != null)
                        {
                            y.InboxStatus = rNode.Status;
                            y.InboxTimeStamp = rNode.CreatedAt;
                        }
                        else
                        {
                            y.InboxTimeStamp = DateTime.MaxValue;
                            y.InboxStatus = -99;
                        }
                        y.EncryptedId = Utilities.Encrypt(y.Id.ToString());
                    }
                    x.RotationUsers = x.RotationUsers.OrderBy(i => i.InboxTimeStamp).ToList();
                }
                return result;
            }
        }
        /// <summary>
        /// Function to count all Rotation status of a Company from Models
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public int CountAllRelatedToCompany(long companyId, ICollection<string> tags)
        {
            using (var db = new ServiceContext())
            {
                var data = (from rotation in db.Rotations
                            where rotation.CompanyId == companyId
                            orderby rotation.UpdatedAt descending
                            select new RotationDashboard
                            {
                                Id = rotation.Id,
                                Subject = rotation.Name,
                                Status = rotation.Status,
                                DateCreated = rotation.CreatedAt,
                                DateUpdated = rotation.UpdatedAt,
                                DateStarted = rotation.StartedAt,
                                Tags = (from tagitem in rotation.TagItems
                                        join tag in db.Tags on tagitem.TagId equals tag.Id
                                        select tag.Name.ToLower()).ToList(),
                                RotationUsers = (from rtuser in rotation.RotationUsers
                                                 select new RotationDashboard.UserDashboard
                                                 {
                                                     Id = rtuser.User.Id,
                                                     Name = rtuser.User.Name,
                                                     ImageProfile = rtuser.User.ProfileImageFileName
                                                 }).ToList(),
                                Creator = (from user in db.Users
                                           where user.Id == rotation.CreatorId
                                           select new RotationDashboard.UserDashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               ImageProfile = user.ProfileImageFileName
                                           }).FirstOrDefault(),
                                Workflow = new RotationDashboard.WorkflowDashboard
                                {
                                    Id = rotation.Workflow.Id,
                                    Name = rotation.Workflow.Name
                                }
                            });
                if (tags != null)
                    data = data.Where(item => tags.All(itag => item.Tags.Contains(itag.ToLower())));
                var result = data.ToList().Count();
                return result;
            }
        }
        public string Delete(long id)
        {
            using (var db = new ServiceContext())
            {
                var rotation = db.Rotations.Where(i => i.Id == id).FirstOrDefault();

                //check if rotation exist
                if (rotation == null)return Constant.RotationStatus.NOT_FOUND.ToString();

                //check is already being used or not
                if (rotation.CompanyId.HasValue) return Constant.RotationStatus.ERROR_ROTATION_ALREADY_STARTED.ToString();

                rotation.IsActive = false;
                rotation.UpdatedAt = DateTime.Now;
                db.SaveChanges();
                return Constant.RotationStatus.OK.ToString();
            }
        }

    }
}