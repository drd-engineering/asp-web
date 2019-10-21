using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoRotationMember
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationId { get; set; } // RotationId
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public long? MemberId { get; set; } // MemberId
        public int FlagPermission { get; set; } // FlagPermission

        public string ActivityName { get; set; }
        public string MemberPicture { get; set; }
        public string MemberNumber { get; set; }
        public string MemberName { get; set; }
        public string MemberEmail { get; set; }

        // Foreign keys
        public virtual DtoMember Member { get; set; } // FK_RotationMember_Member
        public virtual DtoRotation Rotation { get; set; } // FK_RotationMember_Rotation
        public virtual DtoWorkflowNode WorkflowNode { get; set; } // FK_RotationMember_WorkflowNode

        public DtoRotationMember()
        {
            FlagPermission = 0;
        }
    }
}
