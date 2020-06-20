﻿using DRD.Models;
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
                     Value = rotationNode.Value,
                     WorkflowNodeId = rotationNode.WorkflowNodeId,
                     PrevWorkflowNodeId = rotationNode.PrevWorkflowNodeId,
                     SenderRotationNodeId = rotationNode.SenderRotationNodeId,
                     User = new UserInboxData
                     {
                         Id = rotationNode.User.Id,
                         Name = rotationNode.User.Name,
                         ImageProfile = rotationNode.User.ImageProfile,
                         ImageInitials = rotationNode.User.ImageInitials,
                         ImageSignature = rotationNode.User.ImageSignature,
                         ImageStamp = rotationNode.User.ImageStamp,
                         ImageKtp1 = rotationNode.User.ImageKtp1,
                         ImageKtp2 = rotationNode.User.ImageKtp2,
                     },
                     WorkflowNode = new WorkflowNodeInboxData
                     {
                         Id = rotationNode.WorkflowNode.Id,
                         Caption = rotationNode.WorkflowNode.Caption
                     },
                     //RotationNodeRemarks =
                     //(from rotationNodeRemark in rotationNode.RotationNodeRemarks
                     // select new RotationNodeRemarkInboxData
                     // {
                     //     Id = rotationNodeRemark.Id,
                     //     Remark = rotationNodeRemark.Remark,
                     //     DateStamp = rotationNodeRemark.DateStamp,
                     // }).ToList(),
                 }).ToList();
            
            rotation.AccessType = (int)Constant.AccessType.noAccess;

            foreach (RotationNodeInboxData rotationNode in rotation.RotationNodes)
            {
                //set page access to specific user
                if (rotationNode.User.Id == userId)
                {
                    // responsible access for the current user
                    if (rotationNode.Status.Equals((int)Constant.RotationStatus.Open))
                    {
                        rotation.AccessType = (int)Constant.AccessType.responsible;
                    }
                    //readonly access for other user in node or rotation creator
                    else if (!rotation.AccessType.Equals((int)Constant.AccessType.responsible) || rotation.CreatorId == userId)
                    { 
                        rotation.AccessType = (int) Constant.AccessType.readOnly;
                    }
                }


                // user encrypted id
                rotationNode.User.EncryptedUserId = Utilities.Encrypt(rotationNode.User.Id.ToString());
                // set note for waiting pending member
                if (rotationNode.Status.Equals(Constant.RotationStatus.Pending))
                {
                    rotationNode.Note = "Waiting for action from: ";
                    var rtpending = db.RotationNodes.FirstOrDefault(c => c.Id == rotationNode.Id);
                    var wfnto = rtpending.WorkflowNode.WorkflowNodeLinks.FirstOrDefault(c => c.SymbolCode.Equals(Constant.EnumActivityAction.SUBMIT.ToString()));

                    var nodetos = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == wfnto.WorkflowNodeToId && c.SymbolCode.Equals(Constant.EnumActivityAction.SUBMIT.ToString())).ToList();
                    foreach (WorkflowNodeLink workflowNodeLink in nodetos)
                    {
                        int[] statuses = { (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Revision };
                        if (statuses.Contains(workflowNodeLink.WorkflowNode.RotationNodes.FirstOrDefault().Status))
                            rotationNode.Note += workflowNodeLink.WorkflowNode.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.WorkflowNodeId).User.Name + " | " + workflowNodeLink.WorkflowNode.Caption + ", ";
                    }

                    if (rotationNode.Note.EndsWith(", "))
                        rotationNode.Note = rotationNode.Note.Substring(0, rotationNode.Note.Length - 2);
                }
                // set note for alter link
                else if (rotationNode.Status.Equals((int)Constant.RotationStatus.Open))
                {
                    int alter = (int)Constant.EnumActivityAction.ALTER;
                    var node = db.WorkflowNodeLinks.FirstOrDefault(c => c.WorkflowNodeId == rotationNode.WorkflowNode.Id && c.SymbolCode == alter);

                    if (node != null)
                    {
                        if (node.Operator != null)
                        {
                            var start = rotationNode.CreatedAt;
                            double val = 0;
                            if (Double.TryParse(node.Value, out val))
                            {
                                if (node.Operator.Equals("HOUR"))
                                    start = start.AddHours(val);
                                else
                                    start = start.AddDays(val);
                            }

                            rotationNode.Note = "There is a flow alter on " + start.ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                            rotationNode.Note = "There is a flow alter, there has been no determination of the period.";
                    }
                }

                if (rotationNode.Id == rotation.RotationNodeId && rotation.UserId == rotationNode.User.Id && rotationNode.Status.Equals((int)Constant.RotationStatus.Open) && (rotationNode.PrevWorkflowNodeId != null || rotationNode.SenderRotationNodeId != null))
                {
                    if (rotationNode.PrevWorkflowNodeId != null)
                    {
                        var wfn = db.WorkflowNodes.FirstOrDefault(c => c.Id == rotationNode.PrevWorkflowNodeId);
                        if (wfn.SymbolCode.Equals("PARALLEL"))
                        {
                            var wfnIds = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeToId == wfn.Id).Select(c => c.WorkflowNodeId).ToArray();
                            var rns = db.RotationNodes.Where(c => wfnIds.Contains(c.WorkflowNode.Id)).ToList();
                            List<RotationNodeDocInboxData> listDoc = new List<RotationNodeDocInboxData>();
                            List<RotationNodeUpDocInboxData> listUpDoc = new List<RotationNodeUpDocInboxData>();
                            foreach (RotationNode rnx in rns)
                            {
                                var d = AssignNodeDocs(db, rnx.Id, userId/*rotation.UserId*/, rot.RotationNodeId, docSvr);
                                if (d.Count > 0)
                                    listDoc.AddRange(d);

                                var ud = AssignNodeUpDocs(db, rnx.Id);
                                if (ud.Count > 0)
                                    listUpDoc.AddRange(ud);
                            }
                            if (listDoc.Count > 0)
                                rotationNode.RotationNodeDocs = listDoc;
                            if (listUpDoc.Count > 0)
                                rotationNode.RotationNodeUpDocs = listUpDoc;
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    //rotationNode.RotationNodeDocs = assignNodeDocs(db, rotationNode.Id, rotation.UserId, rot.RotationNodeId);
                    rotationNode.RotationNodeDocs = AssignNodeDocs(db, rotationNode.Id, userId/*rotationNode.UserId*/, rot.RotationNodeId, docSvr);
                    rotationNode.RotationNodeUpDocs = AssignNodeUpDocs(db, rotationNode.Id);
                }

                //document summaries document
                foreach (RotationNodeDocInboxData rotationNodeDoc in rotationNode.RotationNodeDocs)
                {
                    // get anno
                    foreach (DocumentElementInboxData documentElement in rotationNodeDoc.Document.DocumentElements)
                    {
                        if (documentElement.ElementId == null || documentElement.ElementId == 0) continue;
                        if (documentElement.ElementTypeId == DocumentService.GetElementTypeFromCsvByCode("SIGNATURE").Id || documentElement.ElementTypeId == DocumentService.GetElementTypeFromCsvByCode("INITIAL").Id || documentElement.ElementTypeId == DocumentService.GetElementTypeFromCsvByCode("PRIVATESTAMP").Id)
                        {
                            var user = db.Users.FirstOrDefault(c => c.Id == documentElement.ElementId);
                            Element newElement = new Element();
                            newElement.EncryptedUserId = Utilities.Encrypt(user.Id.ToString());
                            newElement.UserId = user.Id;
                            newElement.Name = user.Name;
                            newElement.Foto = user.ImageProfile;
                            documentElement.Element = newElement;
                        }
                        else if (documentElement.ElementTypeId == DocumentService.GetElementTypeFromCsvByCode("STAMP").Id)
                        {
                            // Stamp Masih Perlu perbaikan nantinya
                            var stmp = db.Stamps.FirstOrDefault(c => c.Id == documentElement.ElementId);
                            documentElement.Element.EncryptedUserId = Utilities.Encrypt(documentElement.ElementId.ToString());
                            documentElement.Element.Name = stmp.Descr;
                            documentElement.Element.Foto = stmp.StampFile;
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
                //attchment summaries
                //foreach (RotationNodeUpDoc rotationNodeDoc in rotationNode.RotationNodeUpDocs)
                //{
                //    var dx = rotation.SumRotationNodeUpDocs.FirstOrDefault(c => c.Document.FileName.Equals(rotationNodeDoc.Document.FileName));
                //    if (dx == null)
                //        rotation.SumRotationNodeUpDocs.Add(rotationNodeDoc);
                //}
            }
            return rotation;
        }
        /// <summary>
        /// Count Rotation related to the criteria, and user as the creator.
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="topCriteria"></param>
        /// <returns></returns>
        public int FindRotationCountAll(long creatorId, string topCriteria)
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
                                  where rotation.CreatorId == creatorId && (topCriteria.Equals("") || tops.All(criteria => (rotation.Subject).ToLower().Contains(criteria.ToLower())))
                                  orderby rotation.Status, rotation.DateCreated descending, rotation.Subject descending
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
        public ICollection<RotationData> FindRotations(long creatorId, string topCriteria, int skip, int take)
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
                                where rotation.CreatorId == creatorId && (topCriteria.Equals("") || tops.All(criteria => (rotation.Subject).ToLower().Contains(criteria.ToLower())))
                                orderby rotation.Status, rotation.DateCreated descending, rotation.Subject descending
                                select new RotationData
                                {
                                    Id = rotation.Id,
                                    Subject = rotation.Subject,
                                    Status = rotation.Status,
                                    WorkflowId = rotation.Workflow.Id,
                                    WorkflowName = rotation.Workflow.Name,
                                    CompanyId = rotation.CompanyId,
                                    UserId = rotation.UserId,
                                    CreatedAt = rotation.DateCreated,
                                    UpdatedAt = rotation.DateUpdated,
                                    DateStarted = rotation.DateUpdated,
                                }).Skip(skip).Take(take).ToList();
                foreach (RotationData resultItem in result)
                {
                    resultItem.StatusDescription = constant.getRotationStatusName(resultItem.Status);
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
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         Remark = rotation.Remark,
                         UserId = rotation.UserId,
                         DateCreated = rotation.DateCreated,
                         DateUpdated = rotation.DateUpdated,
                         DateStarted = rotation.DateStarted,
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
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;
            }
        }

        public IEnumerable<Rotation> GetByUserId(long userId, string status)
        {
            using (var db = new ServiceContext())
            {
                //string[] statuses = Convert.ToInt32(status.Split(','));
                var statuses = status.Split(',').Select(Int32.Parse).ToList();

                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && statuses.Contains(rotation.Rotation.Status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();

                //var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.User.Id == userId) && rotation.Status.Equals(status)).ToList();
                var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.UserId == userId) && statuses.Contains(rotation.Status)).ToList();

                List<Rotation> rotations = new List<Rotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;
            }
        }

        public IEnumerable<Rotation> GetInboxByUserId(long userId)
        {
            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && rotation.Status.Equals((int)Constant.RotationStatus.Open)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => Ids.Contains(rotation.Id) && rotation.Status.Equals((int)Constant.RotationStatus.In_Progress)).ToList();

                List<Rotation> rotations = new List<Rotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;
            }
        }

        public RotationInboxData GetNodeById(long id)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotationNode in db.RotationNodes
                     select new RotationInboxData
                     {
                         Id = rotationNode.Rotation.Id,
                         Subject = rotationNode.Rotation.Subject,
                         Status = rotationNode.Status,
                         UserId = rotationNode.User.Id,
                         DateCreated = rotationNode.CreatedAt,
                         DateUpdated = rotationNode.UpdatedAt,
                         DateStarted = rotationNode.Rotation.DateUpdated,
                         RotationNodeId = id,
                         DefWorkflowNodeId = rotationNode.WorkflowNode.Id,
                         FlagAction = 0,
                         DecissionInfo = "",
                         StatusDescription = constant.getRotationStatusName(rotationNode.Status),
                         Workflow = new Workflow
                         {
                             Id = rotationNode.Rotation.Workflow.Id,
                             Name = rotationNode.Rotation.Workflow.Name,
                         },
                     }).FirstOrDefault();

                result = AssignNodes(db, result, result.UserId.Value, new DocumentService());

                var workflowNodeLinks = db.WorkflowNodeLinks.Where(rotation => rotation.WorkflowNodeId == result.DefWorkflowNodeId).ToList();
                foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
                {
                    if (workflowNodeLink.SymbolCode.Equals("SUBMIT"))
                    {
                        result.FlagAction |= (int)Constant.EnumActivityAction.SUBMIT;
                        if (workflowNodeLink.WorkflowNodeTo.SymbolCode.Equals("DECISION"))
                            result.DecissionInfo = "Value " + workflowNodeLink.WorkflowNodeTo.Operator + " " + workflowNodeLink.WorkflowNodeTo.Value;
                        else if (workflowNodeLink.WorkflowNodeTo.SymbolCode.Equals("CASE"))
                            result.DecissionInfo = "Expression: " + workflowNodeLink.WorkflowNodeTo.Value;
                    }
                    else if (workflowNodeLink.SymbolCode.Equals("REJECT"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.REJECT;
                    else if (workflowNodeLink.SymbolCode.Equals("REVISI"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.REVISI;
                    else if (workflowNodeLink.SymbolCode.Equals("ALTER"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.ALTER;
                }

                return result;
            }
        }

        public IEnumerable<Rotation> GetNodeByUserId(long userId, string status)
        {
            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && rotation.Status.Equals(status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => Ids.Contains(rotation.Id) && rotation.Status.Equals(Constant.RotationStatus.In_Progress)).ToList();

                List<Rotation> rotations = new List<Rotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;
            }
        }

        public IEnumerable<RotationData> GetNodeByUserId(long userId, string status, string topCriteria, int page, int pageSize)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "Status, DateCreated desc";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotationNode in db.RotationNodes
                     where rotationNode.Rotation.Status.Equals(Constant.RotationStatus.In_Progress) &&
                            rotationNode.UserId == userId && rotationNode.Status.Equals(status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotationNode.Rotation.Subject).ToLower().Contains(RotationUser.ToLower())))
                     select new RotationData
                     {
                         Id = rotationNode.Rotation.Id,
                         RotationNodeId = rotationNode.Id,
                         Subject = rotationNode.Rotation.Subject,
                         Status = rotationNode.Status,
                         WorkflowId = rotationNode.Rotation.Workflow.Id,
                         WorkflowName = rotationNode.Rotation.Workflow.Name,
                         ActivityName = rotationNode.WorkflowNode.Caption,
                         UserId = rotationNode.UserId,
                         StatusDescription = constant.getRotationStatusName(rotationNode.Status),
                         CreatedAt = rotationNode.CreatedAt,
                         UpdatedAt = rotationNode.UpdatedAt,
                         DateStarted = rotationNode.Rotation.DateUpdated,
                     }).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationData rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;
            }
        }
        /// <summary>
        /// Obtain rotation that user has already made and search by Id of the rotation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        public RotationIndex GetRotationById(long id, long creatorId)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.Id == id && rotation.CreatorId == creatorId
                     select new RotationIndex
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Remark = rotation.Remark,
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
                                              Picture = (x.UserId == null ? "icon_user.png" : x.User.ImageProfile),
                                              FlagPermission = x.FlagPermission,
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
            using (var db = new ServiceContext())
            {
                var result =
                    (from workflowNode in db.WorkflowNodes
                     where workflowNode.WorkflowId == workflowId && workflowNode.SymbolCode == 5
                     select new RotationUserData
                     {
                         ActivityName = workflowNode.Caption,
                         WorkflowNodeId = workflowNode.Id,
                         UserId = workflowNode.UserId.Value
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
                            item.Picture = user.ImageProfile;
                        }
                    }
                }
                result = result.OrderBy(c=> c.WorkflowNodeId).ToList();
                return result;
            }
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

        private List<RotationNodeDocInboxData> AssignNodeDocs(ServiceContext db, long rnId, long memId, long? curRnId, IDocumentService docSvr)
        {
            //rotationNode.RotationNodeDocs =
            var result =
                (from d in db.RotationNodeDocs
                 where d.RotationNode.Id == rnId
                 select new RotationNodeDocInboxData
                 {
                     Id = d.Id,
                     FlagAction = d.FlagAction,
                     DocumentId = d.Document.Id,
                     RotationNode = new RotationNodeInboxData
                     {
                         //this line error bcs of extent5.rotation_id does not exist
                         RotationId = d.RotationNode.Rotation.Id,
                     },
                     Document = new DocumentInboxData
                     {
                         Id = d.Document.Id,
                         Extention = d.Document.Extention,
                         FileUrl = d.Document.FileUrl,
                         FileName = d.Document.FileName,
                         FileSize = d.Document.FileSize,
                         // The original documentusers is also commented
                         DocumentUser =
                             (from dm in d.Document.DocumentUsers
                              where dm.UserId == memId // default inbox member
                              select new DocumentUserInboxData
                              {
                                  Id = dm.Id,
                                  DocumentId = dm.DocumentId,
                                  UserId = dm.UserId,
                                  FlagAction = dm.FlagAction,
                                  FlagPermission = dm.FlagPermission,
                              }).FirstOrDefault(),
                         DocumentElements =
                             (from documentElement in d.Document.DocumentElements
                              select new DocumentElementInboxData
                              {
                                  Id = documentElement.Id,
                                  //Document = documentElement.Document,
                                  Page = documentElement.Page,
                                  LeftPosition = documentElement.LeftPosition,
                                  TopPosition = documentElement.TopPosition,
                                  WidthPosition = documentElement.WidthPosition,
                                  HeightPosition = documentElement.HeightPosition,
                                  Color = documentElement.Color,
                                  BackColor = documentElement.BackColor,
                                  Data = documentElement.Data,
                                  Data2 = documentElement.Data2,
                                  Rotation = documentElement.Rotation,
                                  ScaleX = documentElement.ScaleX,
                                  ScaleY = documentElement.ScaleY,
                                  TransitionX = documentElement.TransitionX,
                                  TransitionY = documentElement.TransitionY,
                                  StrokeWidth = documentElement.StrokeWidth,
                                  Opacity = documentElement.Opacity,
                                  Flag = documentElement.Flag,
                                  FlagCode = documentElement.FlagCode,
                                  FlagDate = documentElement.FlagDate,
                                  FlagImage = documentElement.FlagImage,
                                  CreatorId = documentElement.CreatorId,
                                  ElementId = documentElement.ElementId,
                                  UserId = documentElement.UserId,
                                  CreatedAt = documentElement.CreatedAt,
                                  UpdatedAt = documentElement.UpdatedAt,
                                  ElementTypeId = documentElement.ElementTypeId
                                  //ElementType = new ElementTypeInboxData
                                  //{
                                  //    Id = documentElement.ElementType.Id,
                                  //    Code = documentElement.ElementType.Code,
                                  //}
                              }).ToList(),
                     }
                 }).ToList();

            if (result != null /*&& curRnId != 0*/)
            {
                // assign permission
                //DocumentService docSvr = new DocumentService();
                foreach (RotationNodeDocInboxData rnd in result)
                {
                    if (rnd.Document.DocumentUser == null)
                    {
                        rnd.Document.DocumentUser = new DocumentUserInboxData();
                        rnd.Document.DocumentUser.UserId = memId;
                        rnd.Document.DocumentUser.DocumentId = (long)rnd.Document.Id;
                        rnd.Document.DocumentUser.FlagPermission = 6; //default permission read document
                    }
                    if (curRnId == 0)
                        curRnId = -rnd.RotationNode.Rotation.Id;
                    rnd.Document.DocumentUser.FlagPermission |= docSvr.GetPermission(memId, curRnId.Value, (long)rnd.Document.Id);
                }
            }
            return result;// rotationNode.RotationNodeDocs;
        }

        private List<RotationNodeUpDocInboxData> AssignNodeUpDocs(ServiceContext db, long rnId)
        {
            //rotationNode.RotationNodeUpDocs =
            var result =
                (from ud in db.RotationNodeUpDocs
                 where ud.RotationNode.Id == rnId
                 select new RotationNodeUpDocInboxData
                 {
                     Id = ud.Id,
                     DocumentId = ud.DocumentId,
                     //Document = new Document
                     //{
                     //    FileFlag = ud.Document.FileFlag,
                     //    FileName = ud.Document.FileName,
                     //    FileSize = ud.Document.FileSize,
                     //    CreatorId = ud.Document.CreatorId,
                     //    DateCreated = ud.Document.DateCreated,
                     //}
                 }).ToList();

            return result;
        }

        /// <summary>
        /// Function to obtain Rotation status of a Company from Models
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="tags">filter by tag, if tags is empty get all the rotation status data from models</param>
        /// <param name="skip"></param>
        /// <param name="pageSize"> if the pagesize is negative so it will get all the rotation status data from models</param>
        /// <returns></returns>
        public ICollection<RotationDashboard> GetRelatedToCompany(long companyId, ICollection<string> tags, int skip, int pageSize)
        {
            using (var db = new ServiceContext())
            {
                var data = (from rotation in db.Rotations
                            where rotation.CompanyId == companyId
                            orderby rotation.DateUpdated descending
                            select new RotationDashboard
                            {
                                Id = rotation.Id,
                                Subject = rotation.Subject,
                                Status = rotation.Status,
                                DateCreated = rotation.DateCreated,
                                DateUpdated = rotation.DateUpdated,
                                DateStarted = rotation.DateStarted,
                                Tags = (from tagitem in rotation.TagItems
                                        join tag in db.Tags on tagitem.TagId equals tag.Id
                                        select tag.Name.ToLower()).ToList(),
                                RotationUsers = (from rtuser in rotation.RotationUsers
                                                 select new RotationDashboard.UserDashboard
                                                 {
                                                     Id = rtuser.User.Id,
                                                     Name = rtuser.User.Name,
                                                     ImageProfile = rtuser.User.ImageProfile
                                                 }).ToList(),
                                Creator = (from user in db.Users
                                           where user.Id == rotation.CreatorId
                                           select new RotationDashboard.UserDashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               ImageProfile = user.ImageProfile
                                           }).FirstOrDefault(),
                                Workflow = new RotationDashboard.WorkflowDashboard
                                {
                                    Id = rotation.Workflow.Id,
                                    Name = rotation.Workflow.Name
                                }
                            });
                if(tags != null)
                    data = data.Where(item => tags.All(itag => item.Tags.Contains(itag.ToLower())));
                if (pageSize > 0 && skip >= 0)
                    data = data.Skip(skip).Take(pageSize);
                var result = data.ToList();
                foreach (RotationDashboard x in result)
                {
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
                            orderby rotation.DateUpdated descending
                            select new RotationDashboard
                            {
                                Id = rotation.Id,
                                Subject = rotation.Subject,
                                Status = rotation.Status,
                                DateCreated = rotation.DateCreated,
                                DateUpdated = rotation.DateUpdated,
                                DateStarted = rotation.DateStarted,
                                Tags = (from tagitem in rotation.TagItems
                                        join tag in db.Tags on tagitem.TagId equals tag.Id
                                        select tag.Name.ToLower()).ToList(),
                                RotationUsers = (from rtuser in rotation.RotationUsers
                                                 select new RotationDashboard.UserDashboard
                                                 {
                                                     Id = rtuser.User.Id,
                                                     Name = rtuser.User.Name,
                                                     ImageProfile = rtuser.User.ImageProfile
                                                 }).ToList(),
                                Creator = (from user in db.Users
                                           where user.Id == rotation.CreatorId
                                           select new RotationDashboard.UserDashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               ImageProfile = user.ImageProfile
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

        /*public IEnumerable<RotationData> GetLiteAll(long userId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(userId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<RotationData> GetLiteAll(long userId, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order)
        {
            return GetLiteAll(userId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<RotationData> GetLiteAll(long userId, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order, Expression<Func<RotationData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "Status, DateCreated desc";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.User.Id == userId).ToList();
                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                var result =
                    (from rotation in db.Rotations
                     where (Ids.Contains(rotation.Id) || rotation.UserId == userId) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.UserId,
                         StatusDescription = constant.getRotationStatusName(rotation.Status),
                         CreatedAt = rotation.DateCreated,
                         UpdatedAt = rotation.DateUpdated,
                         DateStarted = rotation.DateUpdated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;
            }
        }
        public long GetLiteAllCount(long userId, string topCriteria)
        {
            return GetLiteAllCount(userId, topCriteria, null);
        }
        public long GetLiteAllCount(long userId, string topCriteria, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.User.Id == userId).ToList();
                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();

                var result =
                    (from rotation in db.Rotations
                     where //_appZoneAccess.Contains(rotation.AppZone) &&
                            (Ids.Contains(rotation.Id) || rotation.UserId == userId) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new Rotation
                     {
                         Id = rotation.Id,
                     }).Count();

                return result;
            }
        }
        public IEnumerable<RotationData> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize)
        {
            return GetLiteStatusAll(userId, status, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<RotationData> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order)
        {
            return GetLiteStatusAll(userId, status, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<RotationData> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order, Expression<Func<RotationData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "Status, DateCreated desc";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where statuses.Contains(rotation.Status) &&
                            (rotation.UserId == userId || rotation.RotationNodes.Any(RotationUser => RotationUser.User.Id == userId)) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.UserId,
                         StatusDescription = constant.getRotationStatusName(rotation.Status),
                         CreatedAt = rotation.DateCreated,
                         UpdatedAt = rotation.DateCreated,
                         DateStarted = rotation.DateStarted,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationData rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;
            }
        }
        public long GetLiteStatusAllCount(long userId, string status, string topCriteria)
        {
            return GetLiteStatusAllCount(userId, status, topCriteria, null);
        }
        public long GetLiteStatusAllCount(long userId, string status, string topCriteria, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.User.Id == userId).ToList();
                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                var statuses = status.Split(',').Select(Int32.Parse).ToList();
                var result =
                    (from rotation in db.Rotations
                     where statuses.Contains(rotation.Status) &&
                            (Ids.Contains(rotation.Id) || rotation.UserId == userId) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new Rotation
                     {
                         Id = rotation.Id,
                     }).Count();

                return result;
            }
        }
        public IEnumerable<RotationData> GetNodeLiteAll(long userId, string status, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order, Expression<Func<RotationData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "IsTemplate desc, Name";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.RotationNodes
                     where rotation.User.Id == userId && statuses.Contains(rotation.Status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                         Subject = rotation.Rotation.Subject,
                         Status = rotation.Rotation.Status,
                         WorkflowId = rotation.Rotation.Workflow.Id,
                         WorkflowName = rotation.Rotation.Workflow.Name,
                         ActivityName = rotation.WorkflowNode.Caption,
                         StatusDescription = constant.getRotationStatusName(rotation.Status),
                         UserId = rotation.User.Id,
                         CreatedAt = rotation.CreatedAt,
                         UpdatedAt = rotation.UpdatedAt,
                         DateStarted = rotation.DateRead,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationData rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;
            }
        }
        public IEnumerable<RotationData> GetNodeLiteAll(long userId, string status, string topCriteria, int page, int pageSize)
        {
            return GetNodeLiteAll(userId, status, topCriteria, page, pageSize, null, null);
        }
        public long GetNodeLiteAllCount(long userId, string status, string topCriteria, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.RotationNodes
                     where rotation.User.Id == userId && statuses.Contains(rotation.Status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                     }).Count();

                return result;
            }
        }
        public long GetNodeLiteAllCount(long userId, string status, string topCriteria)
        {
            return GetNodeLiteAllCount(userId, status, topCriteria, null);
        }*/

    }
}