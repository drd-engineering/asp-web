using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonDashboard
    {
        public int UnreadChat { get; set; }

        public int Rotation { get; set; }
        public int Inbox { get; set; }
        public int Altered { get; set; }
        public int Revised { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
        public int Signed { get; set; }
        public int Declined { get; set; }
        public int Completed { get; set; }

        public int InviteAccepted { get; set; }
        public int Invitation { get; set; }

        public decimal DepositBalance { get; set; }
    }
}
