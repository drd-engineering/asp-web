using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.app.Models
{
    public class RotationMember
    {
        public long Id { get; set; } // Id (Primary key)
        public int FlagPermission { get; set; } // FlagPermission

        public string ActivityName { get; set; }
        public string MemberPicture { get; set; }
        public string MemberNumber { get; set; }
        public string MemberName { get; set; }
        public string MemberEmail { get; set; }

        // Foreign keys
        public virtual Member Member { get; set; } // FK_RotationMember_Member
        public virtual Rotation Rotation { get; set; } // FK_RotationMember_Rotation
        public virtual WorkflowNode WorkflowNode { get; set; } // FK_RotationMember_WorkflowNode

        public RotationMember()
        {
            FlagPermission = 0;
        }
    }
}
