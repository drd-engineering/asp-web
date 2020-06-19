using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    // inbox/inboxLisst
    public class InboxList
    {
        public long Id { get; set; }
        public bool IsUnread { get; set; }
        public long CompanyId { get; set; }
        public String RotationName { get; set; } // access rotation activity first then the rotation to get the name
        public String CurrentActivity { get; set; } // rotationActivity name
        public String WorkflowName { get; set; }
        public DateTime CreatedAt { get; set; }
        public String Message { get; set; }
        public long RotationId { get; set; }
        public String DateNote { get; set; }
        public String LastStatus { get; set; }
        public string prevUserName { get; set; }
        public string prevUserEmail { get; set; }
        public SmallCompanyData CompanyInbox { get; set; }
    }
}
