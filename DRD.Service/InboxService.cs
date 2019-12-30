using DRD.Models.Custom;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var inboxes = db.Inboxes.Where(inbox => inbox.UserId == user.Id && inbox.IsUnread).ToList();

                    List<InboxList> result = new List<InboxList>();

                    foreach (Inbox i in inboxes)
                    {
                        InboxList item = new InboxList();
                        item.Id = i.Id;
                        item.IsUnread = i.IsUnread;

                        //var activity = db.RotationActivities.Where(a => a.Id == i.ActivityId).FirstOrDefault();

                        //item.CurrentActivity = activity.Name;
                        //item.RotationName = activity.Workflow.Subject;
                        //item.WorkflowName = activity.Workflow.Workflow.Name;

                        result.Add(item);
                    }
                    return result;
                }
                return null;
            }
        
        }

        public Rotation GetInboxItem(long rotationNodeId, long inboxId, long UserId=0)
        {

            using (var db = new ServiceContext())
            {
                var result =
                   (from c in db.RotationNodes
                    where c.Id == rotationNodeId
                    select new Rotation
                    {
                        Id = c.Rotation.Id,
                        Subject = c.Rotation.Subject,
                        Status = c.Status,
                        WorkflowId = c.Rotation.WorkflowId,
                        UserId = c.MemberId,
                        DateCreated = c.CreatedAt,
                        DateUpdated = c.UpdatedAt,
                        DateStarted = c.DateRead,
                        RotationNodeId = c.Id,
                        StatusDescription = Constant.getRotationStatusNameByCode(c.Status),

                    }).FirstOrDefault();

                RotationService rotationService = new RotationService();
                result = rotationService.assignNodes(db, result, UserId, new DocumentService());

                var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == result.DefWorkflowNodeId).ToList();
                foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
                {
                    if (workflowNodeLink.SymbolCode.Equals("SUBMIT"))
                    {
                        result.FlagAction |= (int)Constant.EnumActivityAction.SUBMIT;
                        if (workflowNodeLink.WorkflowNodeTos.SymbolCode.Equals("DECISION"))
                            result.DecissionInfo = "Value " + workflowNodeLink.WorkflowNodeTos.Operator + " " + workflowNodeLink.WorkflowNodeTos.Value;
                        else if (workflowNodeLink.WorkflowNodeTos.SymbolCode.Equals("CASE"))
                            result.DecissionInfo = "Expression: " + workflowNodeLink.WorkflowNodeTos.Value;
                    }
                    else if (workflowNodeLink.SymbolCode.Equals("REJECT"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.REJECT;
                    else if (workflowNodeLink.SymbolCode.Equals("REVISI"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.REVISI;
                    else if (workflowNodeLink.SymbolCode.Equals("ALTER"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.ALTER;

                }

                changeUnreadtoReadInbox(inboxId: inboxId);

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
                    inboxItem.CreatedAt = DateTime.Now;
                    inboxItem.DateNote = "New Created Inbox from " + activity.RotationName;
                    db.Inboxes.Add(inboxItem);
                    System.Diagnostics.Debug.WriteLine("INBOX MESSAGE :: " + inboxItem.Message);
                    return db.SaveChanges();
                }
            }
            return returnItem;
        }

    }
}
