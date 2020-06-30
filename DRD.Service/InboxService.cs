using DRD.Models;
using DRD.Models.API;
using DRD.Models.View;
using DRD.Service.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class InboxService
    {
        private DocumentService documentService;
        private SymbolService symbolService;
        public InboxService()
        {
            documentService = new DocumentService();
            symbolService = new SymbolService();
        }
        //helper
        private bool CheckIdExist(long id)
        {
            using var db = new ServiceContext();
            return db.Inboxes.Any(i => i.Id == id);
        }


        private void UpdateStatus(ServiceContext db, long rotationId, int previousStatus, int status)
        {
            var rot = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            rot.Status = status;
            rot.UpdatedAt = DateTime.Now;
            foreach (RotationNode rotationNode in rot.RotationNodes)
            {
                if (rotationNode.Status.Equals(previousStatus))
                    rotationNode.Status = status;
            }
        }

        private ActivityItem CreateActivityResult(long userId, long previousUserId, int exitCode, string rotationName, long rotationId, long rotationNodeId, string lastActivityStatus = "")
        {
            using var db = new ServiceContext();
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

        private void InsertDoc(IEnumerable<RotationNodeDoc> docs, ServiceContext db, ref RotationNode rotationNode)
        {
            if (docs != null)
            {
                foreach (RotationNodeDoc rnc in docs)
                {
                    RotationNodeDoc rotationNodeDoc = new RotationNodeDoc
                    {
                        DocumentId = rnc.Document.Id,
                        ActionStatus = rnc.ActionStatus,
                        RotationNodeId = rotationNode.Id,
                        RotationId = rotationNode.RotationId
                    };
                    rotationNode.RotationNodeDocs.Add(rotationNodeDoc);

                    // update flag action di master rotationNodeDoc member
                    var userId = rotationNode.UserId;
                    var rotationid = rotationNode.RotationId;
                    var docm = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == rnc.Document.Id && c.UserId == userId);
                    var rotationUser = db.RotationUsers.FirstOrDefault(rtUsr => rtUsr.RotationId == rotationid && rtUsr.UserId == userId);
                    if (docm != null)
                    {
                        if (((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REMOVE) == (int)Constant.EnumDocumentAction.REMOVE) ||
                            ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REVISI) == (int)Constant.EnumDocumentAction.REVISI))
                            documentService.DocumentRemovedorRevisedFromRotation(rnc.DocumentId);
                        else if (docm.ActionStatus != rnc.ActionStatus) documentService.DocumentUpdatedByRotation(rnc.DocumentId);
                        docm.ActionStatus |= rnc.ActionStatus;
                        // Also Document permission updating related to Rotation User that have permission
                        docm.ActionPermission |= rotationUser.ActionPermission;
                    }
                    else
                    {
                        DocumentUser docmem = new DocumentUser();
                        docmem.DocumentId = rnc.Document.Id;
                        docmem.UserId = userId;
                        if (((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REMOVE) == (int)Constant.EnumDocumentAction.REMOVE) ||
                            ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REVISI) == (int)Constant.EnumDocumentAction.REVISI))
                            documentService.DocumentRemovedorRevisedFromRotation(rnc.DocumentId);
                        else if (rnc.ActionStatus != 0) documentService.DocumentUpdatedByRotation(rnc.DocumentId); docmem.ActionStatus |= rnc.ActionStatus;
                        docmem.ActionPermission = 6; // default view, add annotate
                        // Also Document permission updating related to Rotation User that have permission
                        docmem.ActionPermission |= rotationUser.ActionPermission;
                        db.DocumentUsers.Add(docmem);
                    }

                    if (rnc.Document != null)
                    { // save annos first before set sign/initial/stamp
                        ICollection<DocumentElementInboxData> docElement = new List<DocumentElementInboxData>();
                        foreach (DocumentAnnotation x in rnc.Document.DocumentElements)
                        {
                            docElement.Add(new DocumentElementInboxData(x));
                        }
                        docElement = documentService.SaveAnnos(rnc.Document.Id, userId, "", docElement);
                    }
                    if ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.SIGN) == (int)Constant.EnumDocumentAction.SIGN)
                        documentService.Signature((long)rnc.Document.Id, userId, rotationNode.Rotation.Id);
                    if ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.PRIVATESTAMP) == (int)Constant.EnumDocumentAction.PRIVATESTAMP)
                        documentService.Stamp((long)rnc.Document.Id, userId, rotationNode.Rotation.Id);
                }
                db.SaveChanges();
            }
        }


        /// <summary>
        /// Obtain all inbox data related to user as many as pageSize
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// 
        public List<InboxList> GetInboxes(long userId, string criteria, int skip, int take)
        {
            // all criteria
            string[] allCriteria = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
            {
                allCriteria = criteria.Split(' ');
            }
            else
                criteria = "";
            using (var db = new ServiceContext())
            {
                if (db.Inboxes != null)
                {
                    var inboxes = db.Inboxes.Where(inbox => inbox.UserId == userId && 
                        (criteria.Equals("") || allCriteria.All(crtr => (inbox.Message).ToLower().Contains(
                            crtr.ToLower())))).ToList().OrderByDescending(item => item.CreatedAt).Skip(skip).Take(take);
                    List<InboxList> result = new List<InboxList>();
                    foreach (Inbox i in inboxes)
                    {
                        InboxList item = new InboxList();
                        item.Id = i.Id;
                        item.IsUnread = i.IsUnread;

                        var activity = db.RotationNodes.Where(a => a.Id == i.ActivityId).FirstOrDefault();

                        item.CurrentActivity = activity.WorkflowNode.Caption;
                        item.RotationName = activity.Rotation.Name;
                        item.RotationId = activity.RotationId;
                        item.CompanyId = activity.Rotation.CompanyId.Value;
                        item.Message = i.Note;
                        item.LastStatus = i.LastStatus;
                        item.prevUserEmail = i.PreviousUserEmail;
                        item.prevUserName = i.PreviousUserName;
                        item.DateNote = i.Message;
                        item.WorkflowName = activity.WorkflowNode.Workflow.Name;
                        item.CompanyInbox = (from cmpny in db.Companies
                                             where cmpny.Id == item.CompanyId
                                             select new SmallCompanyData
                                             {
                                                 Id = cmpny.Id,
                                                 Code = cmpny.Code,
                                                 Name = cmpny.Name,
                                             }).FirstOrDefault();
                        result.Add(item);
                    }
                    return result;
                }
                return null;
            }
        }
        /// <summary>
        /// Count all inbox related to criteria. If criteria empty it will return all the inbox
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public int CountInboxes(long userId, string criteria)
        {
            // all criteria
            string[] allCriteria = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
            {
                allCriteria = criteria.Split(' ');
            }
            else
                criteria = "";
            using (var db = new ServiceContext())
            {
                if (db.Inboxes != null)
                {
                    var inboxesCount = db.Inboxes.Where(inbox => inbox.UserId == userId &&
                        (criteria.Equals("") || allCriteria.All(crtr => (inbox.Message).ToLower().Contains(
                            crtr.ToLower())))).ToList().Count();
                    return inboxesCount;
                }
                return 0;
            }

        }
        /// <summary>
        /// Count Inbox that still not read by user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CountUnreadInboxes(long userId)
        {
            using (var db = new ServiceContext())
            {
                if (db.Inboxes != null)
                {
                    var inboxesCount = db.Inboxes.Where(inbox => inbox.UserId == userId && inbox.IsUnread).ToList().Count();

                    return inboxesCount;
                }
                return 0;
            }

        }
        /// <summary>
        /// Helper function to know id of rotation that inbox is attached
        /// </summary>
        /// <param name="inboxId"></param>
        /// <returns></returns>
        public long GetRotationNodeId(long inboxId)
        {
            using var db = new ServiceContext();
            if (db.Inboxes != null)
            {
                var inbox = db.Inboxes.Where(i => i.Id == inboxId).FirstOrDefault();
                return inbox.ActivityId;
            }
            return -1;
        }

        /// <summary>
        /// Find inbox details based on userid and inboxid
        /// </summary>
        /// <param name="inboxId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public RotationInboxData GetInbox(long inboxId, long UserId)
        {
            ChangeUnreadtoReadInbox(inboxId);
            var rotationNodeId = GetRotationNodeId(inboxId);
            using var db = new ServiceContext();
            var result =
                (from c in db.RotationNodes
                where c.Id == rotationNodeId
                select new RotationInboxData
                {
                Id = c.Rotation.Id,
                Name = c.Rotation.Name,
                CreatorId = c.Rotation.CreatorId,
                Status = c.Status,
                CompanyId = c.Rotation.CompanyId,
                WorkflowId = c.Rotation.WorkflowId,
                UserId = c.UserId,
                FirstNodeId = c.FirstNodeId,
                DefWorkflowNodeId = c.WorkflowNodeId,
                FlagAction = 0,
                DecissionInfo = "",
                RotationNodeId = c.Id,
                }).FirstOrDefault();
            result.CompanyInbox = (from cmpny in db.Companies
                                   where cmpny.Id == result.CompanyId
                                   select new SmallCompanyData
                                   {
                                       Id = cmpny.Id,
                                       Code = cmpny.Code,
                                       Name = cmpny.Name,
                                   }).FirstOrDefault();
            var tagService = new TagService();
            var tags = tagService.GetTags(result.Id);
            foreach (var tag in tags) { result.Tags.Add(tag.Name); }

            RotationService rotationService = new RotationService();
            result = AssignNodes(db, result, UserId);

            var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.SourceId == result.DefWorkflowNodeId).ToList();
            foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
            {
                if (workflowNodeLink.SymbolCode.Equals("SUBMIT"))
                    result.FlagAction |= (int)Constant.EnumActivityAction.SUBMIT;
                else if (workflowNodeLink.SymbolCode.Equals("REJECT"))
                    result.FlagAction |= (int)Constant.EnumActivityAction.REJECT;
                else if (workflowNodeLink.SymbolCode.Equals("REVISI"))
                    result.FlagAction |= (int)Constant.EnumActivityAction.REVISI;
                else if (workflowNodeLink.SymbolCode.Equals("ALTER"))
                    result.FlagAction |= (int)Constant.EnumActivityAction.ALTER;

            }
            /*changeUnreadtoReadInbox(inboxId: inboxId);*/
            return result;
        }
        /// <summary>
        /// Mark inbox as read
        /// </summary>
        /// <param name="inboxId">id of inbox that want to marked as read</param>
        /// <returns></returns>
        public bool ChangeUnreadtoReadInbox(long inboxId)
        {
            using var db = new ServiceContext();
            var inbox = db.Inboxes.Where(i => i.Id == inboxId).FirstOrDefault();
            inbox.IsUnread = false;
            db.SaveChanges();
            return inbox.IsUnread;
        }
        public int CreateInbox(ActivityItem activity)
        {
            int returnItem = -1;
            if (activity.ExitCode > 0)
            {
                using var db = new ServiceContext();
                Inbox inboxItem;
                inboxItem = new Inbox();
                while (CheckIdExist(inboxItem.Id))
                {
                    inboxItem.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                }
                inboxItem.IsUnread = true;
                if (activity.RotationNodeId < 0)
                {
                    inboxItem.Note = "You success to start a Rotation";
                    var userResponsible = db.Users.FirstOrDefault(user => user.Id == activity.UserId);
                    if (userResponsible != null)
                    {
                        inboxItem.UserId = activity.UserId;
                    }
                    else { return -1; }
                }
                else
                {
                    inboxItem.Note = activity.UserName + ", you has new Work on Rotatiion " + activity.RotationName;
                    var activityItem = db.RotationNodes.FirstOrDefault(rtNode => rtNode.Id == activity.RotationNodeId);
                    if (activityItem != null)
                    {
                        inboxItem.ActivityId = activity.RotationNodeId;
                    }
                    else { return -1; }
                    var userResponsible = db.Users.FirstOrDefault(user => user.Id == activity.UserId);
                    if (userResponsible != null)
                    {
                        inboxItem.UserId = activity.UserId;
                    }
                    else { return -1; }
                }

                if (activity.LastActivityStatus != null && activity.LastActivityStatus.Equals("SUBMIT"))
                {
                    inboxItem.Message = "You need to review " + activity.RotationName;
                    inboxItem.LastStatus = "REVIEW";
                    UpdatePreviousInbox(activity);
                }
                else
                {
                    inboxItem.LastStatus = "UPLOAD";
                    inboxItem.Message = "New Created Inbox from " + activity.RotationName;
                    if (activity.PreviousUserId != activity.UserId)
                    {
                        Inbox inboxItem2;
                        inboxItem2 = new Inbox();
                        while (inboxItem2.Id == inboxItem.Id || CheckIdExist(inboxItem2.Id))
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
                inboxItem.PreviousUserEmail = activity.PreviousEmail;
                inboxItem.PreviousUserName = activity.PreviousUserName;
                inboxItem.RotationId = activity.RotationId;
                inboxItem.CreatedAt = DateTime.Now;
                db.Inboxes.Add(inboxItem);
                var dbsave = db.SaveChanges();
                SendEmailActivity(inboxItem);
                return dbsave;
            }
            return returnItem;
        }
        public int UpdatePreviousInbox(ActivityItem activity)
        {
            using (var db = new ServiceContext())
            {
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
        }
        public int GenerateNewInbox(ActivityItem activity)
        {
            using (var db = new ServiceContext())
            {
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
        }
        public List<ActivityItem> ProcessActivity(ProcessActivity parameter, Constant.EnumActivityAction bit)
        {
            List<ActivityItem> retvalues = new List<ActivityItem>();

            using (var db = new ServiceContext())
            {
                var strbit = bit.ToString();

                //get current rotation node
                RotationNode rtnode = db.RotationNodes.FirstOrDefault(c => c.Id == parameter.RotationNodeId);

                //set last node to in progress
                rtnode.Status = (int)Constant.RotationStatus.In_Progress;
                rtnode.UpdatedAt = DateTime.Now;
                rtnode.Rotation.StartedAt = DateTime.Now;
                InsertDoc(parameter.RotationNodeDocs, db, ref rtnode);

                if (strbit.Equals("REVISI"))
                {
                    rtnode.Status = (int)Constant.RotationStatus.Revision;

                    var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.SourceId == rtnode.WorkflowNodeId).FirstOrDefault();
                    RotationNode rtnode2 = new RotationNode
                    {
                        RotationId = rtnode.RotationId,
                        WorkflowNodeId = workflowNodeLink.FirstNodeId,
                        FirstNodeId = rtnode.FirstNodeId,
                        SenderRotationNodeId = rtnode.Id,
                        PreviousWorkflowNodeId = workflowNodeLink.SourceId,// tested OK
                        Status = (int)Constant.RotationStatus.Open,
                        UserId = (long)workflowNodeLink.FirstNode.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.FirstNodeId && c.RotationId == rtnode.RotationId).User.Id
                    };
                    db.RotationNodes.Add(rtnode2);
                    db.SaveChanges();
                    //TODO change how to get last id inserted
                    long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                    retvalues.Add(CreateActivityResult(rtnode2.UserId, rtnode.UserId, (int)Constant.RotationStatus.Revision, rtnode2.Rotation.Name, rtnode2.RotationId, lastProductId, strbit));
                }
                else if (strbit.Equals("REJECT"))
                {
                    var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.SourceId == rtnode.WorkflowNodeId).FirstOrDefault();
                    rtnode.Status = (int)Constant.RotationStatus.Declined;
                    UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);

                    retvalues.Add(CreateActivityResult(rtnode.UserId, rtnode.UserId, (int)Constant.RotationStatus.Declined, rtnode.Rotation.Name, rtnode.RotationId, rtnode.Id, strbit));
                }
                else if (strbit.Equals("SUBMIT"))
                {
                    Symbol symbol = symbolService.getSymbol(strbit);
                    int symbolCode = symbol == null ? 0 : symbol.Id;
                    var wfnodes = db.WorkflowNodeLinks.Where(c => c.SourceId == rtnode.WorkflowNodeId && c.SymbolCode == symbolCode).ToList();
                    List<RotationNode> rotnodes = new List<RotationNode>();

                    foreach (WorkflowNodeLink workflowNodeLink in wfnodes)
                    {
                        var nodeto = workflowNodeLink.Target;

                        if (nodeto.SymbolCode == symbolService.getSymbolId("ACTIVITY"))
                        {
                            RotationNode rtnode2 = db.RotationNodes.Create<RotationNode>();

                            rtnode2.RotationId = rtnode.RotationId;
                            rtnode2.WorkflowNodeId = workflowNodeLink.TargetId;
                            rtnode2.FirstNodeId = rtnode.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            rtnode2.UserId = (long)workflowNodeLink.Target.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.TargetId && c.RotationId == rtnode.RotationId).User.Id;
                            rtnode2.PreviousWorkflowNodeId = workflowNodeLink.SourceId;// tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.CreatedAt = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);

                            db.SaveChanges();
                            //TODO change how to get last id inserted
                            long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                            retvalues.Add(CreateActivityResult(rtnode2.UserId, rtnode.UserId, (int)Constant.RotationStatus.In_Progress, rtnode2.Rotation.Name, rtnode2.RotationId, lastProductId, strbit));
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("END"))
                        {
                            if (rtnode.Status.Equals((int)Constant.RotationStatus.Declined))
                                UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);
                            else
                                UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);
                            retvalues.Add(CreateActivityResult(rtnode.UserId, rtnode.UserId, 1, rtnode.Rotation.Name, rtnode.RotationId, rtnode.Id, "END"));
                        }
                    }
                }
                var result = db.SaveChanges();

                InboxService inboxService = new InboxService();
                foreach (ActivityItem act in retvalues)
                {
                    inboxService.GenerateNewInbox(act);
                }
                return retvalues;
            }
        }
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
            using (var db = new ServiceContext())
            {
                user = db.Users.FirstOrDefault(c => c.Id == inbox.UserId);
            }

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_SENDER_}", inbox.PreviousUserName + " (" + inbox.PreviousUserEmail + ")");
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_ACTION_}", inbox.LastStatus);
            body = body.Replace("{_MESSAGE_}", inbox.Message);

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

            System.Diagnostics.Debug.WriteLine(senderEmail + senderName + user.Email + "Inbox Reception");

            var task = emailService.Send(senderEmail, senderName, user.Email, "You have a task in DRD", body, false, new string[] { });
        }


        private RotationInboxData AssignNodes(ServiceContext db, RotationInboxData rot, long userId)
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

                rotationNode.RotationNodeDocs = AssignNodeDocs(db, rotationNode.Id, userId, rot.RotationNodeId);

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
        /// Obtain all the Rotation Node Document will return as inbox data
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rnId"></param>
        /// <param name="usrId"></param>
        /// <param name="curRnId"></param>
        /// <param name="documentService"></param>
        /// <returns></returns>
        private List<RotationNodeDocInboxData> AssignNodeDocs(ServiceContext db, long rnId, long usrId, long? curRnId)
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
                item.Document.Extension = rndDb.Document.Extension;
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
                    dUsrItem.FlagPermission |= documentService.GetPermission(usrId, curRnId.Value, dusr.DocumentId);
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

        private static RotationNodeDocInboxData DeepCopy(RotationNodeDocInboxData source)
        {
            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<RotationNodeDocInboxData>(JsonConvert.SerializeObject(source), DeserializeSettings);
        }
    }
}
