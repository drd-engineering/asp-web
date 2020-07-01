using System;

namespace DRD.Models.API
{
    // inbox/inboxLisst
    public class InboxListItem
    {
        public long Id { get; set; }
        public bool IsUnread { get; set; }
        public long CompanyId { get; set; }
        public String RotationName { get; set; } // access rotation activity first then the rotation to get the name
        public String CurrentActivity { get; set; } // rotationActivity name
        public String WorkflowName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public String Message { get; set; }
        public long RotationId { get; set; }
        public String Note { get; set; }
        public String LastStatus { get; set; }
        public string PreviousUserName { get; set; }
        public string PreviousUserEmail { get; set; }
        public SmallCompanyData CompanyInbox { get; set; }
    }
}
