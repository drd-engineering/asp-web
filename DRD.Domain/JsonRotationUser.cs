using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonRotationUser
    {
        public long WorkflowNodeId { get; set; }
        public long MemberId { get; set; }

        public string ActivityName { get; set; }
        public string MemberName { get; set; }
    }
}
