using DRD.Models.Custom;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.API.List;

namespace DRD.Service
{   
    public class InboxService
    {

        public void GetInboxList(UserSession user) {
            using (var db = new ServiceContext()) 
            { 

                var inboxes = db.Inboxes.Where(inbox => inbox.UserId == user.Id).ToList();

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
            }
        
        }

        public void GetInboxItem(UserSession user, long inboxId)
        {
            List<RotationItem> rotationLog = new List<RotationItem>();
            // should update isUnread
            using (var db = new ServiceContext())
            {
                var inbox = db.Inboxes.Where(i => i.UserId == user.Id && i.Id == inboxId).FirstOrDefault();

                InboxItem result = new InboxItem();
                
                result.Id = inbox.Id;
                //result.CurrentActivity = inbox.Activity.Name;

                rotationLog = (
                    from r in db.Rotations
                    //join ra in db.RotationActivities on r.Id equals ra.WorkflowId
                    //where ra.WorkflowId == r.Id
                    select new RotationItem { 
                    
                        // lanjutin
                        // ambil dari rotation
                    }
                    
                ).ToList();

            }

        }
    }
}
