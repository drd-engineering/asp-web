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

                        var activity = db.RotationNodes.Where(a => a.Id == i.ActivityId).FirstOrDefault();

                        item.CurrentActivity = activity.WorkflowNode.Caption;
                        item.RotationName = activity.Rotation.Subject;
                        item.WorkflowName = activity.WorkflowNode.Workflow.Name;
                        item.CreatedAt = i.CreatedAt;

                        result.Add(item);
                    }
                    return result;
                }
                return null;
            }
        
        }

        public InboxItem GetInboxItem(UserSession user, long inboxId)
        {
            List<RotationData> rotationLog = new List<RotationData>();
            List<DocumentItem> documentList = new List<DocumentItem>();
            // should update isUnread
            using (var db = new ServiceContext())
            {
                var inbox = db.Inboxes.Where(i => i.UserId == user.Id && i.Id == inboxId).FirstOrDefault();

                InboxItem result = new InboxItem();
                
                result.Id = inbox.Id;
                //result.CurrentActivity = inbox.Activity.Name;

                // Rotation Log
                rotationLog = (
                    from r in db.Rotations
                    join rn in db.RotationNodes on r.Id equals rn.RotationId
                    where rn.Id == inbox.ActivityId
                    orderby r.DateUpdated descending
                    select new RotationData { 
                    
                        // lanjutin
                        // ambil dari rotation
                    }
                    
                ).ToList();
                result.RotationLog = rotationLog;

                // Document
                documentList = (
                    from doc in db.Documents
                    where doc.Rotation.Id == inbox.Activity.RotationId
                    select new DocumentItem
                    {
                        Id = doc.Id,
                        Title = doc.Title,
                        FileNameOri = doc.FileName
                    }
                ).ToList();
                result.Documents = documentList;

                return result;

            }

        }
    }
}
