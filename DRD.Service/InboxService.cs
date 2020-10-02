using DRD.Models;
using DRD.Models.API;
using DRD.Models.View;
using DRD.Service.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRD.Service
{
    public class InboxService
    {
        private DocumentService documentService;
        private SymbolService symbolService;
        private CompanyService companyService;

        public InboxService()
        {
            documentService = new DocumentService();
            symbolService = new SymbolService();
        }

        /// <summary>
        /// HELPER
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckIdExist(long id)
        {
            using var db = new Connection();
            return db.Inboxes.Any(i => i.Id == id);
        }

        /// <summary>
        /// UPDATE status of rotation from previous to the new status
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rotationId"></param>
        /// <param name="previousStatus"></param>
        /// <param name="newStatus"></param>
        private void UpdateStatus(Connection db, long rotationId, int previousStatus, int newStatus)
        {
            var rotationDb = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            rotationDb.Status = newStatus;
            foreach (RotationNode rotationNode in rotationDb.RotationNodes)
            {
                if (rotationNode.Status.Equals(previousStatus))
                    rotationNode.Status = newStatus;
            }
        }

        /// <summary>
        /// CREATE result of rotation processing
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="previousUserId"></param>
        /// <param name="exitCode"></param>
        /// <param name="rotationName"></param>
        /// <param name="rotationId"></param>
        /// <param name="rotationNodeId"></param>
        /// <param name="lastActivityStatus"></param>
        /// <returns></returns>
        private ActivityItem CreateActivityResult(long userId, long previousUserId, int exitCode, string rotationName, long rotationId, long rotationNodeId, string lastActivityStatus = "")
        {
            using var db = new Connection();
            var mem = db.Users.FirstOrDefault(c => c.Id == userId);
            var prev = db.Users.FirstOrDefault(c => c.Id == previousUserId);
            ActivityItem ret = new ActivityItem
            {
                RotationId = rotationId,
                ExitCode = exitCode,
                UserId = userId,
                UserName = mem.Name,
                Email = mem.Email,

                PreviousUserId = previousUserId,
                PreviousUserName = prev.Name,
                PreviousEmail = prev.Email,

                RotationName = rotationName,
                RotationNodeId = rotationNodeId,
                LastActivityStatus = lastActivityStatus
            };
            ret.ExitStatus ??= Constant.RotationStatus.OK.ToString();
            return ret;
        }

        /// <summary>
        /// INSERT all document from document list to the rotation node with document permission, action status, annotation, etc.
        /// </summary>
        /// <param name="docs"></param>
        /// <param name="db"></param>
        /// <param name="rotationNode"></param>
        private void InsertDoc(IEnumerable<RotationNodeDoc> docs, Connection db, ref RotationNode rotationNode)
        {
            if (docs == null) return;
            var userId = rotationNode.UserId;
            var rotationid = rotationNode.RotationId;
            var rotationNodeId = rotationNode.Id;
            foreach (RotationNodeDoc rotationNodeDoc in docs)
            {
                var newRotationNodeDoc = db.RotationNodeDocs.FirstOrDefault(item => item.DocumentId == rotationNodeDoc.DocumentId && item.RotationNodeId == rotationNodeId && item.RotationId == rotationid);
                if (newRotationNodeDoc == null)
                {
                    newRotationNodeDoc = new RotationNodeDoc
                    {
                        DocumentId = rotationNodeDoc.DocumentId,
                        ActionStatus = rotationNodeDoc.ActionStatus,
                        RotationNodeId = rotationNode.Id,
                        RotationId = rotationNode.RotationId
                    };
                    rotationNode.RotationNodeDocs.Add(newRotationNodeDoc);
                    db.RotationNodeDocs.Add(newRotationNodeDoc);
                }
                newRotationNodeDoc.ActionStatus = rotationNodeDoc.ActionStatus;
                // update flag action di master rotationNodeDoc member
                var documentUserDb = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == rotationNodeDoc.DocumentId && c.UserId == userId);
                var rotationUserDb = db.RotationUsers.FirstOrDefault(rtUsr => rtUsr.RotationId == rotationid && rtUsr.UserId == userId);

                if (documentUserDb == null)
                {
                    documentUserDb = documentService.CreateDocumentUser(rotationNodeDoc.DocumentId, userId);
                }
                if (((rotationNodeDoc.ActionStatus & (int)Models.Constant.EnumDocumentAction.REMOVE) == (int)Models.Constant.EnumDocumentAction.REMOVE) ||
                    ((rotationNodeDoc.ActionStatus & (int)Models.Constant.EnumDocumentAction.REVISI) == (int)Models.Constant.EnumDocumentAction.REVISI))
                    documentService.DocumentRemovedorRevisedFromRotation(rotationNodeDoc.DocumentId);
                else if (documentUserDb.ActionStatus != rotationNodeDoc.ActionStatus) documentService.DocumentUpdatedByRotation(rotationNodeDoc.DocumentId);
                documentUserDb.ActionStatus |= rotationNodeDoc.ActionStatus;
                documentUserDb.ActionPermission |= rotationUserDb.ActionPermission;
                db.SaveChanges();

                if (rotationNodeDoc.Document == null) continue;
                var annoToSave = new List<DocumentAnnotationsInboxData>();
                foreach(DocumentAnnotation da in rotationNodeDoc.Document.DocumentAnnotations)
                {
                    annoToSave.Add(new DocumentAnnotationsInboxData(da));
                }
                documentService.SaveAnnos(rotationNodeDoc.DocumentId, userId, "", annoToSave);

                if ((rotationNodeDoc.ActionStatus & (int)Models.Constant.EnumDocumentAction.SIGN) == (int)Models.Constant.EnumDocumentAction.SIGN)
                    documentService.Signature((long)rotationNodeDoc.DocumentId, userId, rotationNode.RotationId);
                if ((rotationNodeDoc.ActionStatus & (int)Models.Constant.EnumDocumentAction.PRIVATESTAMP) == (int)Models.Constant.EnumDocumentAction.PRIVATESTAMP)
                    documentService.Stamp((long)rotationNodeDoc.DocumentId, userId, rotationNode.RotationId);
            }
            db.SaveChanges();
        }

        /// <summary>
        /// GET all inbox data related to user as many as pageSize
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        ///
        public List<InboxListItem> GetInboxes(long userId, string criteria, int skip, int take)
        {
            // all criteria
            string[] criterias = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
                criterias = criteria.Split(' ');
            else
                criteria = "";

            using var db = new Connection();
            if (db.Inboxes == null) return null;

            var inboxesDb = db.Inboxes.Where(inbox => inbox.UserId == userId &&
                    (criteria.Equals("") || criterias.All(crtr => (inbox.Message).ToLower().Contains(
                        crtr.ToLower())))).ToList().OrderByDescending(item => item.CreatedAt).Skip(skip).Take(take);
            List<InboxListItem> result = new List<InboxListItem>();

            foreach (Inbox inbox in inboxesDb)
            {
                var activity = db.RotationNodes.Where(a => a.Id == inbox.ActivityId).FirstOrDefault();
                InboxListItem inboxListItem = new InboxListItem
                {
                    Id = inbox.Id,
                    IsUnread = inbox.IsUnread,

                    CurrentActivity = activity.WorkflowNode.Caption,
                    RotationName = activity.Rotation.Name,
                    RotationId = activity.RotationId,
                    CompanyId = activity.Rotation.CompanyId.Value,
                    Note = inbox.Note,
                    LastStatus = inbox.LastStatus,
                    PreviousUserEmail = inbox.PreviousUserEmail,
                    PreviousUserName = inbox.PreviousUserName,
                    Message = inbox.Message,
                    WorkflowName = activity.WorkflowNode.Workflow.Name,
                    CreatedAt = inbox.CreatedAt
                };
                inboxListItem.CompanyInbox = (from company in db.Companies
                                              where company.Id == inboxListItem.CompanyId
                                              select new SmallCompanyData
                                              {
                                                  Id = company.Id,
                                                  Code = company.Code,
                                                  Name = company.Name,
                                              }).FirstOrDefault();
                result.Add(inboxListItem);
            }
            return result;
        }

        /// <summary>
        /// GET the total inbox related to criteria. If criteria empty it will return all the inbox
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public int CountInboxes(long userId, string criteria)
        {
            // all criteria
            string[] allCriteria = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
                allCriteria = criteria.Split(' ');
            else
                criteria = "";

            using var db = new Connection();
            if (db.Inboxes != null)
                return db.Inboxes.Count(inbox => inbox.UserId == userId &&
                    (criteria.Equals("") || allCriteria.All(crtr => (inbox.Message).ToLower().Contains(
                        crtr.ToLower()))));

            return 0;
        }

        /// <summary>
        /// GET total Inbox that still not read by user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CountUnreadInboxes(long userId)
        {
            using var db = new Connection();
            if (db.Inboxes != null) return db.Inboxes.Count(inbox => inbox.UserId == userId && inbox.IsUnread);
            return 0;
        }

        /// <summary>
        /// HELPER function to know id of rotation that inbox is attached
        /// </summary>
        /// <param name="inboxId"></param>
        /// <returns></returns>
        public long GetRotationNodeId(long inboxId)
        {
            using var db = new Connection();
            if (db.Inboxes != null)
                return db.Inboxes.Where(i => i.Id == inboxId).FirstOrDefault().ActivityId;
            return -1;
        }

        /// <summary>
        /// GET inbox details based on userid and inboxid
        /// </summary>
        /// <param name="inboxId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public RotationInboxData GetInbox(long inboxId, long UserId)
        {
            SetInboxToRead(inboxId);
            var rotationNodeId = GetRotationNodeId(inboxId);
            using var db = new Connection();
            var result =
                (from rotationNode in db.RotationNodes
                 where rotationNode.Id == rotationNodeId
                 select new RotationInboxData()
                 {
                     Id = rotationNode.Rotation.Id,
                     Name = rotationNode.Rotation.Name,
                     CreatorId = rotationNode.Rotation.CreatorId,
                     CompanyId = rotationNode.Rotation.CompanyId,
                     WorkflowId = rotationNode.Rotation.WorkflowId,
                     ActionStatus = 0,
                     Status = rotationNode.Status,
                     RotationStatus = rotationNode.Rotation.Status,
                     UserId = rotationNode.UserId,
                     FirstNodeId = rotationNode.FirstNodeId,
                     CurrentActivity = rotationNode.WorkflowNodeId,
                     RotationNodeId = rotationNode.Id,
                 }).FirstOrDefault();
            companyService = new CompanyService();
            result.CompanyInbox = companyService.GetCompany(result.CompanyId.Value);
            var tagService = new TagService();
            result.Tags = tagService.GetTagsAsString(result.Id);

            RotationService rotationService = new RotationService();
            AssignNodes(db, result, UserId);

            var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.SourceId == result.CurrentActivity).ToList();
            foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
            {
                if (workflowNodeLink.SymbolCode.Equals("SUBMIT"))
                    result.ActionStatus |= (int)Models.Constant.EnumActivityAction.SUBMIT;
                else if (workflowNodeLink.SymbolCode.Equals("REJECT"))
                    result.ActionStatus |= (int)Models.Constant.EnumActivityAction.REJECT;
                else if (workflowNodeLink.SymbolCode.Equals("REVISI"))
                    result.ActionStatus |= (int)Models.Constant.EnumActivityAction.REVISI;
            }
            return result;
        }

        /// <summary>
        /// SAVE status inbox as read
        /// </summary>
        /// <param name="inboxId">id of inbox that want to marked as read</param>
        /// <returns></returns>
        private bool SetInboxToRead(long inboxId)
        {
            using var db = new Connection();
            var inbox = db.Inboxes.Where(i => i.Id == inboxId).FirstOrDefault();
            inbox.IsUnread = false;
            db.SaveChanges();
            return inbox.IsUnread;
        }

        /// <summary>
        /// CREATE new inbox depends on rotation process result
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public int CreateInbox(ActivityItem activity)
        {
            //abort if thre is an error
            if (activity.ExitCode <= 0) return activity.ExitCode;

            using var db = new Connection();
            Inbox inboxDb = new Inbox();
            while (CheckIdExist(inboxDb.Id))
            {
                inboxDb.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
            }
            inboxDb.IsUnread = true;

            if (activity.RotationNodeId < 0)
            {
                inboxDb.Note = "You success to start a Rotation";
                var userResponsible = db.Users.FirstOrDefault(user => user.Id == activity.UserId);
                if (userResponsible != null)
                {
                    inboxDb.UserId = activity.UserId;
                }
                else { return -1; }
            }
            else
            {
                inboxDb.Note = activity.UserName + ", you has new Work on Rotatiion " + activity.RotationName;
                var activityItem = db.RotationNodes.FirstOrDefault(rtNode => rtNode.Id == activity.RotationNodeId);
                if (activityItem != null)
                {
                    inboxDb.ActivityId = activity.RotationNodeId;
                }
                else { return -1; }
                var userResponsible = db.Users.FirstOrDefault(user => user.Id == activity.UserId);
                if (userResponsible != null)
                {
                    inboxDb.UserId = activity.UserId;
                }
                else { return -1; }
            }

            if (activity.LastActivityStatus != null && activity.LastActivityStatus.Equals("SUBMIT"))
            {
                inboxDb.Message = "You need to review " + activity.RotationName;
                inboxDb.LastStatus = "REVIEW";
                UpdatePreviousInbox(activity);
            }
            else
            {
                inboxDb.LastStatus = "UPLOAD";
                inboxDb.Message = "New Created Inbox from " + activity.RotationName;
                if (activity.PreviousUserId != activity.UserId)
                {
                    Inbox inboxItem2;
                    inboxItem2 = new Inbox();
                    while (inboxItem2.Id == inboxDb.Id || CheckIdExist(inboxItem2.Id))
                    {
                        inboxItem2.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                    }
                    inboxItem2.IsUnread = true;
                    inboxItem2.LastStatus = "ROTATION";
                    inboxItem2.Message = "You have rotation - " + activity.RotationName;
                    inboxItem2.PreviousUserEmail = activity.PreviousEmail;
                    inboxItem2.PreviousUserName = activity.PreviousUserName;
                    inboxItem2.UserId = activity.PreviousUserId;
                    inboxItem2.ActivityId = activity.RotationNodeId;
                    inboxItem2.RotationId = activity.RotationId;
                    inboxItem2.CreatedAt = DateTime.Now;
                    db.Inboxes.Add(inboxItem2);
                    SendEmailActivity(inboxItem2);
                }
            }
            inboxDb.PreviousUserEmail = activity.PreviousEmail;
            inboxDb.PreviousUserName = activity.PreviousUserName;
            inboxDb.RotationId = activity.RotationId;
            inboxDb.CreatedAt = DateTime.Now;
            db.Inboxes.Add(inboxDb);
            var dbsave = db.SaveChanges();
            SendEmailActivity(inboxDb);
            return dbsave;
        }

        /// <summary>
        /// UPDATE previous inbox depends on rotation processing result
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public int UpdatePreviousInbox(ActivityItem activity)
        {
            using var db = new Connection();
            var prevInbox = db.Inboxes.Where(item => item.RotationId == activity.RotationId && item.UserId != activity.UserId).ToList();
            if (prevInbox != null)
            {
                foreach (Inbox inbox in prevInbox)
                {
                    if (activity.LastActivityStatus.Equals("SUBMIT"))
                    {
                        inbox.Message = activity.UserName + "(" + activity.Email + ")" + " is reviewing " + activity.RotationName;
                        inbox.LastStatus = "INFO";
                    }
                    else if (activity.LastActivityStatus.Equals("REVISI"))
                    {
                        inbox.Message = activity.UserName + "(" + activity.Email + ")" + " is revising " + activity.RotationName;
                        inbox.LastStatus = "INFO";
                    }
                    else if (activity.LastActivityStatus.Equals("REJECT"))
                    {
                        inbox.Message = "This " + activity.RotationName + " has been rejected by " + activity.PreviousUserName + "(" + activity.PreviousEmail + ")";
                        inbox.LastStatus = "REJECTED";
                    }
                    else if (activity.LastActivityStatus.Equals("END"))
                    {
                        inbox.Message = "This " + activity.RotationName + " has been completed";
                        inbox.LastStatus = "COMPLETED";
                    }
                    inbox.CreatedAt = DateTime.Now;
                    inbox.PreviousUserEmail = activity.PreviousEmail;
                    inbox.PreviousUserName = activity.PreviousUserName;
                }
                return db.SaveChanges();
            }
            return -1;
        }

        /// <summary>
        /// CREATE new inbox from rotation processing response
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public int GenerateNewInbox(ActivityItem activity)
        {
            using var db = new Connection();
            var inbox = db.Inboxes.Where(item => item.RotationId == activity.RotationId && item.UserId == activity.UserId).FirstOrDefault();
            if (inbox != null)
            {
                inbox.ActivityId = activity.RotationNodeId;
                inbox.CreatedAt = DateTime.Now;
                inbox.PreviousUserEmail = activity.PreviousEmail;
                inbox.PreviousUserName = activity.PreviousUserName;
                inbox.IsUnread = true;

                if (activity.LastActivityStatus.Equals("SUBMIT"))
                {
                    inbox.Message = "You need to review " + activity.RotationName;
                    inbox.LastStatus = "REVIEW";
                    SendEmailActivity(inbox);
                }
                else if (activity.LastActivityStatus.Equals("REVISI"))
                {
                    inbox.Message = "You need to revise " + activity.RotationName;
                    inbox.LastStatus = "REVISION";
                    SendEmailActivity(inbox);
                }
                else if (activity.LastActivityStatus.Equals("REJECT"))
                {
                    inbox.Message = "This " + activity.RotationName + " has ben rejected by you";
                    inbox.LastStatus = "REJECTED";
                }
                else if (activity.LastActivityStatus.Equals("END"))
                {
                    inbox.Message = "This " + activity.RotationName + " has been completed";
                    inbox.LastStatus = "COMPLETED";
                }
                UpdatePreviousInbox(activity);
                return db.SaveChanges();
            }
            else
            {
                return CreateInbox(activity);
            }
        }

        /// <summary>
        /// UPDATE rotation node status and creating new inbox after that
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public List<ActivityItem> ProcessActivity(ProcessActivity parameter, Models.Constant.EnumActivityAction bit)
        {
            List<ActivityItem> returnValue = new List<ActivityItem>();

            using var db = new Connection();
            var stringBit = bit.ToString();

            //get current rotation node
            RotationNode rotationNode = db.RotationNodes.FirstOrDefault(c => c.Id == parameter.RotationNodeId);

            //set last node to in progress
            rotationNode.Status = (int)Constant.RotationStatus.In_Progress;

            InsertDoc(parameter.RotationNodeDocs, db, ref rotationNode);

            if (stringBit.Equals("REVISI"))
            {
                rotationNode.Status = (int)Constant.RotationStatus.Revision;
                UpdateStatus(db,rotationNode.RotationId,(int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Revision);

                var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.SourceId == rotationNode.WorkflowNodeId).FirstOrDefault();
                RotationNode rotationNode2 = new RotationNode
                {
                    RotationId = rotationNode.RotationId,
                    WorkflowNodeId = workflowNodeLink.FirstNodeId,
                    FirstNodeId = rotationNode.FirstNodeId,
                    SenderRotationNodeId = rotationNode.Id,
                    PreviousWorkflowNodeId = workflowNodeLink.SourceId,// tested OK
                    Status = (int)Constant.RotationStatus.Open,
                    UserId = (long)workflowNodeLink.FirstNode.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.FirstNodeId && c.RotationId == rotationNode.RotationId).UserId
                };
                db.RotationNodes.Add(rotationNode2);
                db.SaveChanges();
                //TODO change how to get last id inserted
                long lastProductId = rotationNode2.Id;
                returnValue.Add(CreateActivityResult(rotationNode2.UserId, rotationNode.UserId, (int)Constant.RotationStatus.Revision, rotationNode.Rotation.Name, rotationNode.RotationId, lastProductId, stringBit));
            }
            else if (stringBit.Equals("REJECT"))
            {
                var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.SourceId == rotationNode.WorkflowNodeId).FirstOrDefault();
                rotationNode.Status = (int)Constant.RotationStatus.Declined;
                UpdateStatus(db, rotationNode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);

                returnValue.Add(CreateActivityResult(rotationNode.UserId, rotationNode.UserId, (int)Constant.RotationStatus.Declined, rotationNode.Rotation.Name, rotationNode.RotationId, rotationNode.Id, stringBit));
            }
            else if (stringBit.Equals("SUBMIT"))
            {
                Symbol symbol = symbolService.getSymbol(stringBit);
                int symbolCode = symbol == null ? 0 : symbol.Id;

/*                var workflowNodeCheckSourceLinksDb = db.WorkflowNodeLinks.Where(c => c.TargetId == rotationNode.WorkflowNodeId && c.SymbolCode == symbolCode).ToList();
                //check if current activity has multiple source
                if (workflowNodeCheckSourceLinksDb.Count > 1)
                {
                    foreach (WorkflowNodeLink workflowNodeLink in workflowNodeCheckSourceLinksDb)
                    {
                        var rotationNodeCheck = db.RotationNodes.FirstOrDefault(rn => rn.WorkflowNodeId == workflowNodeLink.Source.WorkflowId);
                        //if any parallel source target to current still open, pending the current submit
                        if (rotationNodeCheck.Status.Equals(Constant.RotationStatus.Open))
                        {
                            //change current status to pending
                            rotationNode.Status = (int)Constant.RotationStatus.Pending;
                            returnValue.Add(CreateActivityResult(rotationNode.UserId, rotationNode.UserId, (int)Constant.RotationStatus.Pending, rotationNode.Rotation.Name, rotationNode.RotationId, rotationNode.Id, stringBit));
                            return returnValue;
                        }
                    }
                    //change all pending source to next activity
                    foreach (WorkflowNodeLink workflowNodeLink in workflowNodeCheckSourceLinksDb)
                    {
                        var rotationNodeCheck = db.RotationNodes.FirstOrDefault(rn => rn.WorkflowNodeId == workflowNodeLink.Source.WorkflowId);
                        returnValue = ProcessNextSubmitActivity(returnValue, rotationNodeCheck, symbolCode, stringBit);
                    }
                }*/

                //process current Rotation Node
                returnValue = ProcessNextSubmitActivity(returnValue, rotationNode, symbolCode, stringBit);
            }
            var result = db.SaveChanges();

            InboxService inboxService = new InboxService();
            foreach (ActivityItem activity in returnValue)
            {
                inboxService.GenerateNewInbox(activity);
            }
            return returnValue;
        }

        private List<ActivityItem> ProcessNextSubmitActivity(List<ActivityItem> returnValue, RotationNode rotationNode, int symbolCode, string stringBit)
        {
            using var db = new Connection();
            //get workflow node in db
            var workflowNodeLinksDb = db.WorkflowNodeLinks.Where(c => c.SourceId == rotationNode.WorkflowNodeId && c.SymbolCode == symbolCode).ToList();
            List<RotationNode> rotnodes = new List<RotationNode>();

            //loop workflownode in db to spread the workflow if parallel happen
            foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinksDb)
            {
                var nodeto = workflowNodeLink.Target;

                if (nodeto.SymbolCode == symbolService.getSymbolId("ACTIVITY"))
                {

                    //Find all the IDs of source workflow link 
                    var workflownodelinktargetsources = db.WorkflowNodeLinks.Where(c => c.TargetId == workflowNodeLink.TargetId && c.SourceId != workflowNodeLink.SourceId && c.SymbolCode == symbolCode).Select(c => c.SourceId).ToHashSet();

                    //check if any other parallel source whish is still ongoing
                    bool anyUnfinishParallel = db.RotationNodes.Any(rn => rn.RotationId == rotationNode.RotationId && rn.Status == (int)Constant.RotationStatus.Open && workflownodelinktargetsources.Contains(rn.WorkflowNodeId));

                    //check if this next node has already initiated (open/pending)
                    var rotationNodeLookup = db.RotationNodes.FirstOrDefault(rn => rn.RotationId == rotationNode.RotationId && ( rn.Status == (int)Constant.RotationStatus.Open || rn.Status == (int)Constant.RotationStatus.Pending) && rn.WorkflowNodeId == workflowNodeLink.TargetId);
                    
                    //if rotation node never initiated
                    if (rotationNodeLookup == null)
                    {
                        RotationNode rotationNode2 = db.RotationNodes.Create<RotationNode>();
                        //change status to pending, waiting for all parallel sources finished
                        rotationNode2.Status = anyUnfinishParallel? (int)Constant.RotationStatus.Pending:(int)Constant.RotationStatus.Open;

                        rotationNode2.RotationId = rotationNode.RotationId;
                        rotationNode2.WorkflowNodeId = workflowNodeLink.TargetId;
                        rotationNode2.FirstNodeId = rotationNode.FirstNodeId;
                        rotationNode2.SenderRotationNodeId = rotationNode.Id;
                        rotationNode2.UserId = (long)workflowNodeLink.Target.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.TargetId && c.RotationId == rotationNode.RotationId).User.Id;
                        rotationNode2.PreviousWorkflowNodeId = workflowNodeLink.SourceId;// tested OK
                        rotationNode2.CreatedAt = DateTime.Now;

                        rotationNodeLookup = rotationNode2;

                        db.RotationNodes.Add(rotationNode2);
                    }

                    //change status to pending, waiting for all parallel sources finished
                    rotationNodeLookup.Status = anyUnfinishParallel ? (int)Constant.RotationStatus.Pending : (int)Constant.RotationStatus.Open;

                    db.SaveChanges();
                    long lastProductId = rotationNodeLookup.Id;
                    returnValue.Add(CreateActivityResult(rotationNodeLookup.UserId, rotationNode.UserId, (int)Constant.RotationStatus.In_Progress, rotationNodeLookup.Rotation.Name, rotationNodeLookup.RotationId, lastProductId, stringBit));
                }
                else if (nodeto.SymbolCode == symbolService.getSymbolId("END"))
                {
                    //Find all the IDs of source workflow link 
                    var workflownodelinktargetsources = db.WorkflowNodeLinks.Where(c => c.TargetId == workflowNodeLink.TargetId && c.SourceId != workflowNodeLink.SourceId && c.SymbolCode == symbolCode).Select(c => c.SourceId).ToHashSet();

                    //check if any other parallel source whish is still ongoing
                    bool anyUnfinishParallel = db.RotationNodes.Any(rn => rn.RotationId == rotationNode.RotationId && rn.Status == (int)Constant.RotationStatus.Open && workflownodelinktargetsources.Contains(rn.WorkflowNodeId));

                    if (!anyUnfinishParallel)
                    {
                        if (rotationNode.Status.Equals((int)Constant.RotationStatus.Declined))
                            UpdateStatus(db, rotationNode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);
                        else
                            UpdateStatus(db, rotationNode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Completed);
                        returnValue.Add(CreateActivityResult(rotationNode.UserId, rotationNode.UserId, 1, rotationNode.Rotation.Name, rotationNode.RotationId, rotationNode.Id, "END"));
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// SEND email to user about the update rotation processing
        /// </summary>
        /// <param name="inbox"></param>
        public void SendEmailActivity(Inbox inbox)
        {
            var configGenerator = new AppConfigGenerator();
            var topaz = configGenerator.GetConstant("APPLICATION_NAME")["value"];
            var senderName = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();

            string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/InboxNotif.html"));

            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
            var user = new User();
            using (var db = new Connection())
            {
                user = db.Users.FirstOrDefault(c => c.Id == inbox.UserId);
            }

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_SENDER_}", inbox.PreviousUserName + " (" + inbox.PreviousUserEmail + ")");
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_ACTION_}", inbox.LastStatus);
            body = body.Replace("{_MESSAGE_}", inbox.Message);

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

            var task = emailService.Send(senderEmail, senderName, user.Email, "You have a task in DRD", body, false, new string[] { });
        }

        /// <summary>
        /// HELPER assign all rotation node to the rotation
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rotation"></param>
        /// <param name="userId"></param>
        private void AssignNodes(Connection db, RotationInboxData rotation, long userId)
        {
            IEnumerable<RotationNode> rotationNodesDb = db.RotationNodes.Where(item => item.RotationId == rotation.Id).OrderBy(item => item.CreatedAt).ToList();
            //if owner has access to readonly
            rotation.AccessType = rotation.CreatorId == userId ? (int)Models.Constant.AccessType.readOnly : (int)Models.Constant.AccessType.noAccess;
            rotation.DocumentActionPermissionType = rotation.CreatorId == userId ? (int)Models.Constant.DocumentActionPermissionType.FullAccess : (int)Models.Constant.DocumentActionPermissionType.DependsOnRotationUser;
            foreach (RotationNode rotationNodeDb in rotationNodesDb)
            {
                var newRotationNode = new RotationNodeInboxData(rotationNodeDb);
                //set page access to specific user
                if (newRotationNode.User.Id == userId)
                {
                    rotation.AccessType = (int)Models.Constant.AccessType.readOnly;
                    // responsible access for the current user
                    if (newRotationNode.Status.Equals((int)Constant.RotationStatus.Open))
                    {
                        rotation.AccessType = (int)Models.Constant.AccessType.responsible;
                    }
                }
                //document summaries document
                foreach (RotationNodeDocInboxData rotationNodeDoc in newRotationNode.RotationNodeDocs)
                {
                    foreach (DocumentUserInboxData documentUser in rotationNodeDoc.Document.DocumentUsers)
                    {
                        var ru = db.RotationUsers.FirstOrDefault(c => c.UserId == documentUser.UserId && c.RotationId == rotation.Id && c.WorkflowNodeId == newRotationNode.WorkflowNodeId);
                        if (ru != null) documentUser.ActionPermission |= ru.ActionPermission;
                    }
                    var documentUserNow = rotationNodeDoc.Document.DocumentUsers.FirstOrDefault(itmDocUsr => itmDocUsr.UserId == rotation.UserId);
                    // create default docUser For Rotation
                    if (documentUserNow == null)
                    {
                        var duDb = documentService.CreateDocumentUser(rotationNodeDoc.DocumentId, rotation.UserId.Value);
                        documentUserNow = new DocumentUserInboxData(duDb);
                    }
                    rotationNodeDoc.Document.DocumentUser = documentUserNow;
                    // get anno
                    foreach (DocumentAnnotationsInboxData documentElement in rotationNodeDoc.Document.DocumentAnnotations)
                    {
                        if (documentElement.UserId == null || documentElement.UserId == 0) continue;
                        if (documentElement.ElementTypeId == (int)Models.Constant.EnumElementTypeId.SIGNATURE
                            || documentElement.ElementTypeId == (int)Models.Constant.EnumElementTypeId.INITIAL
                            || documentElement.ElementTypeId == (int)Models.Constant.EnumElementTypeId.PRIVATESTAMP)
                        {
                            var user = db.Users.FirstOrDefault(c => c.Id == documentElement.UserId);
                            Element newElement = new Element();
                            newElement.EncryptedUserId = Utilities.Encrypt(user.Id.ToString());
                            newElement.UserId = user.Id;
                            newElement.Name = user.Name;
                            newElement.Foto = user.ProfileImageFileName;
                            documentElement.Element = newElement;
                        }
                    }
                    var dx = rotation.SumRotationNodeDocs.FirstOrDefault(c => c.DocumentId == rotationNodeDoc.DocumentId);
                    if (dx != null)
                    {
                        dx.ActionStatus |= rotationNodeDoc.ActionStatus;
                    }
                    else
                    {
                        rotation.SumRotationNodeDocs.Add(DeepCopy(rotationNodeDoc));
                    }
                }
                rotation.RotationNodes.Add(newRotationNode);
            }
        }

        /// <summary>
        /// HELPER to copy rotation node documnet to a new instance
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static RotationNodeDocInboxData DeepCopy(RotationNodeDocInboxData source)
        {
            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<RotationNodeDocInboxData>(JsonConvert.SerializeObject(source), DeserializeSettings);
        }
    }
}