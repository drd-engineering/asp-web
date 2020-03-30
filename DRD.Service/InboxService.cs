using DRD.Models.Custom;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.View;

namespace DRD.Service
{   
    public class InboxService
    {
        
        public List<InboxList> GetInboxList(UserSession user) {
            using (var db = new ServiceContext()) 
            {
                if (db.Inboxes != null)
                {
                    var inboxes = db.Inboxes.Where(inbox => inbox.UserId == user.Id && inbox.IsUnread).ToList().OrderByDescending(item => item.CreatedAt);

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
                        item.Message = i.Message;
                        item.LastStatus = i.LastStatus;
                        item.prevUserEmail = i.prevUserEmail;
                        item.prevUserName = i.prevUserName;
                        item.DateNote = i.DateNote;
                        item.WorkflowName = activity.WorkflowNode.Workflow.Name;
                        item.CreatedAt = i.CreatedAt;

                        result.Add(item);
                    }
                    return result;
                }
                return null;
            }
        
        }

        public long GetRotationNodeId(long inboxId)
        {
            InboxItem inboxItem = new InboxItem();
            using (var db = new ServiceContext())
            {
                if (db.Inboxes != null)
                {
                    var inbox = db.Inboxes.Where(i =>  i.Id == inboxId).FirstOrDefault();
                    return inbox.ActivityId;
                }
                return -1;
            }
        }

        public InboxItem GetInboxItemById(long inboxId, UserSession user) {
            InboxItem inboxItem = new InboxItem();
            using (var db = new ServiceContext()) 
            {
                if (db.Inboxes != null)
                {
                    var inbox = db.Inboxes.Where(i => i.UserId == user.Id && i.Id == inboxId).FirstOrDefault();

                    System.Diagnostics.Debug.WriteLine("INBOX ID " + inbox.Id);

                    inboxItem.CurrentActivity = db.RotationNodes.Where(rn => rn.Id == inbox.ActivityId).Select(rn => rn.WorkflowNode.Caption).FirstOrDefault();

                    // mapping rotation log
                    inboxItem.RotationLog = (from r in db.Rotations
                                             join rn in db.RotationNodes on r.Id equals rn.RotationId
                                             where rn.Id == inbox.ActivityId
                                             select new RotationData
                                             {
                                                 Id = r.Id,
                                                 Subject = r.Subject,
                                                 WorkflowId = rn.WorkflowNode.Id,
                                        
                                                 Status = rn.Status,
                                                 UserId = rn.UserId,
                                                 //MemberId = 0,
                                                 CreatedAt = rn.CreatedAt,
                                                 UpdatedAt = rn.UpdatedAt,
                                                 //DateStarted,
                                                 //DateStatus,
                                                 RotationNodeId = rn.Id,
                                                 ActivityName = rn.WorkflowNode.Caption,
                                                 WorkflowName = r.Workflow.Name,
                                                 StatusDescription = r.StatusDescription
                                             }
                                             ).ToList();

                    
                    // Un-comment this when inbox feature ready
                    //inbox.IsUnread = false;
                    db.SaveChanges();

                    return inboxItem;
                }
                return null;
            }
        }

        public RotationInboxData GetInboxItem(long inboxId, long UserId=0)
        {
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
                        WorkflowId = c.Rotation.WorkflowId,
                        UserId = c.MemberId,
                        DateCreated = c.CreatedAt,
                        DateUpdated = c.UpdatedAt,
                        DateStarted = c.DateRead,
                        DefWorkflowNodeId = c.WorkflowNodeId,
                        FlagAction = 0,
                        DecissionInfo = "",
                        RotationNodeId = c.Id,

                    }).FirstOrDefault();
                result.StatusDescription = Constant.getRotationStatusNameByCode(result.Status);

                RotationService rotationService = new RotationService();
                result = rotationService.assignNodes(db, result, UserId, new DocumentService());

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

        public bool changeUnreadtoReadInbox(long inboxId)
        {
            using (var db = new ServiceContext())
            {
                var inbox = db.Inboxes.Where(i => i.Id == inboxId).FirstOrDefault();
                inbox.IsUnread = !inbox.IsUnread;
                db.SaveChanges();
                return inbox.IsUnread;
            }

        }

        public int CreateInbox(ActivityItem activity)
        {
            int returnItem = -1;
            System.Diagnostics.Debug.WriteLine("EXIT CODE :: " + activity.ExitCode + " " + activity.UserId + " " + activity.RotationNodeId);
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
                    }
                    inboxItem.prevUserEmail = activity.PreviousEmail;
                    inboxItem.prevUserName = activity.PreviousUserName;
                    inboxItem.RotationId = activity.RotationId;
                    inboxItem.CreatedAt = DateTime.Now;
                    db.Inboxes.Add(inboxItem);
                    System.Diagnostics.Debug.WriteLine("INBOX MESSAGE :: " + inboxItem.Message);
                    return db.SaveChanges();
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
                            inbox.DateNote = "This " + activity.RotationName + " has ben rejected by " + activity.PreviousUserName + "(" + activity.PreviousEmail + ")";
                            inbox.LastStatus = "REJECTED";
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
                System.Diagnostics.Debug.WriteLine("INBOX UPDATE :: " + inbox.Id  + " " + activity.UserId + " " + activity.RotationNodeId + " " + activity.LastActivityStatus);
                    if (activity.LastActivityStatus.Equals("SUBMIT"))
                    {
                          inbox.DateNote = "You need to review " + activity.RotationName;
                          inbox.LastStatus = "REVIEW";

                    }
                    else if (activity.LastActivityStatus.Equals("REVISI"))
                    {
                          inbox.DateNote = "You need to revise " + activity.RotationName;
                          inbox.LastStatus = "REVISION";
                    }
                    else if (activity.LastActivityStatus.Equals("REJECT"))
                    {
                          inbox.DateNote = "This " + activity.RotationName + " has ben rejected by " + activity.PreviousUserName + "(" +  activity.PreviousEmail + "}";
                          inbox.LastStatus = "REJECTED";
                    }

                    inbox.ActivityId = activity.RotationNodeId;
                    inbox.CreatedAt = DateTime.Now;
                    inbox.prevUserEmail = activity.PreviousEmail;
                    inbox.prevUserName = activity.PreviousUserName;
                    UpdatePreviousInbox(activity);
                    return db.SaveChanges();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("INBOX NULL");
                    return CreateInbox(activity);
                }
            }
        }

    }
}
