using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.Custom
{
    public class RotationUserData
    {
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public string ActivityName { get; set; }
        public long? MemberId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }
}
