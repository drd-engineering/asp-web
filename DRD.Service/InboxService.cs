using DRD.Models;
using DRD.Models.API;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class InboxService
    {
        /// <summary>
        /// Obtain all inbox data related to user as many as pageSize
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<InboxList> GetInboxList(long userId, string criteria, int skip, int take)
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
                        (criteria.Equals("") || allCriteria.All(crtr => (inbox.DateNote).ToLower().Contains(
                            crtr.ToLower())))).ToList().OrderByDescending(item => item.CreatedAt).Skip(skip).Take(take);
                    List<InboxList> result = new List<InboxList>();
                    foreach (Inbox i in inboxes)
                    {
                        InboxList item = new InboxList();
                        item.Id = i.Id;
                        item.IsUnread = i.IsUnread;

                        var activity = db.RotationNodes.Where(a => a.Id == i.ActivityId).FirstOrDefault();

                        item.CurrentActivity = activity.WorkflowNode.Caption;
                        item.RotationName = activity.Rotation.Subject;
                        item.RotationId = activity.RotationId;
                        item.CompanyId = activity.Rotation.CompanyId.Value;
                        item.Message = i.Message;
                        item.LastStatus = i.LastStatus;
                        item.prevUserEmail = i.prevUserEmail;
                        item.prevUserName = i.prevUserName;
                        item.DateNote = i.DateNote;
                        item.WorkflowName = activity.WorkflowNode.Workflow.Name;
                        item.CreatedAt = i.CreatedAt;
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
        public int CountAll(long userId, string criteria)
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
                    var inboxesCount = db.Inboxes.Where(inbox => inbox.UserId == userId && &&
                        (criteria.Equals("") || allCriteria.All(crtr => (inbox.DateNote).ToLower().Contains(
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
        public int CountUnread(long userId)
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
            using (var db = new ServiceContext())
            {
                if (db.Inboxes != null)
                {
                    var inbox = db.Inboxes.Where(i => i.Id == inboxId).FirstOrDefault();
                    return inbox.ActivityId;
                }
                return -1;
            }
        }

        /// <summary>
        /// Find inbox details based on userid and inboxid
        /// </summary>
        /// <param name="inboxId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public RotationInboxData GetInboxItem(long inboxId, long UserId)
        {
            ChangeUnreadtoReadInbox(inboxId);
            var rotationNodeId = GetRotationNodeId(inboxId);
            using (var db = new ServiceContext())
            {
                var result =
                   (from c in db.RotationNodes
                    where c.Id == rotationNodeId
                    select new RotationInboxData
                    {
                        Id = c.Rotation.Id,
                        Subject = c.Rotation.Subject,
                        Status = c.Status,
                        CompanyId = c.Rotation.CompanyId,
                        WorkflowId = c.Rotation.WorkflowId,
                        UserId = c.UserId,
                        FirstNodeId = c.FirstNodeId,
                        DateCreated = c.CreatedAt,
                        DateUpdated = c.UpdatedAt,
                        DateStarted = c.DateRead,
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
                result.StatusDescription = Constant.getRotationStatusNameByCode(result.Status);

                RotationService rotationService = new RotationService();
                result = rotationService.AssignNodes(db, result, UserId, new DocumentService());

                var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == result.DefWorkflowNodeId).ToList();
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
                /*changeUnreadtoReadInbox(inboxId: inboxId);*/
                return result;
            }
        }
        /// <summary>
        /// Mark inbox as read
        /// </summary>
        /// <param name="inboxId">id of inbox that want to marked as read</param>
        /// <returns></returns>
        public bool ChangeUnreadtoReadInbox(long inboxId)
        {
            using (var db = new ServiceContext())
            {
                var inbox = db.Inboxes.Where(i => i.Id == inboxId).FirstOrDefault();
                inbox.IsUnread = false;
                db.SaveChanges();
                return inbox.IsUnread;
            }
        }

        public int CreateInbox(ActivityItem activity)
        {
            int returnItem = -1;
            if (activity.ExitCode > 0)
            {
                using (var db = new ServiceContext())
                {
                    Inbox inboxItem;
                    inboxItem = new Inbox();
                    inboxItem.IsUnread = true;
                    if (activity.RotationNodeId < 0)
                    {
                        inboxItem.Message = "You success to start a Rotation";
                        var userResponsible = db.Users.FirstOrDefault(user => user.Id == activity.UserId);
                        if (userResponsible != null)
                        {
                            inboxItem.UserId = activity.UserId;
                        }
                        else { return -1; }
                    }
                    else
                    {
                        inboxItem.Message = activity.UserName + ", you has new Work on Rotatiion " + activity.RotationName;
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
                        inboxItem.DateNote = "You need to review " + activity.RotationName;
                        inboxItem.LastStatus = "REVIEW";
                        UpdatePreviousInbox(activity);
                    }
                    else
                    {
                        inboxItem.LastStatus = "UPLOAD";
                        inboxItem.DateNote = "New Created Inbox from " + activity.RotationName;
                        if (activity.PreviousUserId != activity.UserId)
                        {
                            Inbox inboxItem2;
                            inboxItem2 = new Inbox();
                            inboxItem2.IsUnread = true;
                            inboxItem2.LastStatus = "ROTATION";
                            inboxItem2.DateNote = "You have rotation - " + activity.RotationName;
                            inboxItem2.prevUserEmail = activity.PreviousEmail;
                            inboxItem2.prevUserName = activity.PreviousUserName;
                            inboxItem2.UserId = activity.PreviousUserId;
                            inboxItem2.ActivityId = activity.RotationNodeId;
                            inboxItem2.RotationId = activity.RotationId;
                            inboxItem2.CreatedAt = DateTime.Now;
                            db.Inboxes.Add(inboxItem2);
                            sendemailactivity(inboxItem2);
                        }
                    }
                    inboxItem.prevUserEmail = activity.PreviousEmail;
                    inboxItem.prevUserName = activity.PreviousUserName;
                    inboxItem.RotationId = activity.RotationId;
                    inboxItem.CreatedAt = DateTime.Now;
                    db.Inboxes.Add(inboxItem);
                    var dbsave = db.SaveChanges();
                    sendemailactivity(inboxItem);
                    return dbsave;
                }
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
                            inbox.DateNote = activity.UserName + "(" + activity.Email + ")" + " is reviewing " + activity.RotationName;
                            inbox.LastStatus = "INFO";

                        }
                        else if (activity.LastActivityStatus.Equals("REVISI"))
                        {
                            inbox.DateNote = activity.UserName + "(" + activity.Email + ")" + " is revising " + activity.RotationName;
                            inbox.LastStatus = "INFO";
                        }
                        else if (activity.LastActivityStatus.Equals("REJECT"))
                        {
                            inbox.DateNote = "This " + activity.RotationName + " has been rejected by " + activity.PreviousUserName + "(" + activity.PreviousEmail + ")";
                            inbox.LastStatus = "REJECTED";
                        }
                        else if (activity.LastActivityStatus.Equals("END"))
                        {
                            inbox.DateNote = "This " + activity.RotationName + " has been completed";
                            inbox.LastStatus = "COMPLETED";
                        }
                        inbox.CreatedAt = DateTime.Now;
                        inbox.prevUserEmail = activity.PreviousEmail;
                        inbox.prevUserName = activity.PreviousUserName;
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
                    inbox.prevUserEmail = activity.PreviousEmail;
                    inbox.prevUserName = activity.PreviousUserName;
                    inbox.IsUnread = true;

                    if (activity.LastActivityStatus.Equals("SUBMIT"))
                    {
                        inbox.DateNote = "You need to review " + activity.RotationName;
                        inbox.LastStatus = "REVIEW";
                        sendemailactivity(inbox);
                    }
                    else if (activity.LastActivityStatus.Equals("REVISI"))
                    {
                        inbox.DateNote = "You need to revise " + activity.RotationName;
                        inbox.LastStatus = "REVISION";
                        sendemailactivity(inbox);
                    }
                    else if (activity.LastActivityStatus.Equals("REJECT"))
                    {
                        inbox.DateNote = "This " + activity.RotationName + " has ben rejected by you";
                        inbox.LastStatus = "REJECTED";
                    }
                    else if (activity.LastActivityStatus.Equals("END"))
                    {
                        inbox.DateNote = "This " + activity.RotationName + " has been completed";
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
        public void sendemailactivity(Inbox inbox)
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
            body = body.Replace("{_SENDER_}", inbox.prevUserName + " (" + inbox.prevUserEmail + ")");
            body = body.Replace("{_NAME_}", user.Name);
            body = body.Replace("{_ACTION_}", inbox.LastStatus);
            body = body.Replace("{_MESSAGE_}", inbox.DateNote);

            body = body.Replace("//images", "/images");

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

            System.Diagnostics.Debug.WriteLine(senderEmail + senderName + user.Email + "Inbox Reception");

            var task = emailService.Send(senderEmail, senderName, user.Email, "You have a task in DRD", body, false, new string[] { });
        }
    }
}
