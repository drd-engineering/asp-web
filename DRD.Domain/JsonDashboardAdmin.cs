using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonDashboardAdmin
    {
        public int Inbox { get; set; }

        public int MemberTopupPending { get; set; }
        public int MemberTopupConfirmation { get; set; }
        public int MemberTopupConfirmed { get; set; }
        public int MemberTopupNotConfirmed { get; set; }
    }
}
