using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationMembers", Schema = "public")]
    public class RotationUser
    {
        public long Id { get; set; } // Id (Primary key)
        public int FlagPermission { get; set; } // FlagPermission
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public string ActivityName { get; set; }
        public string Picture { get; set; }
        public long MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // Foreign keys
        public virtual User User { get; set; } // FK_RotationMember_Member
        public virtual Rotation Rotation { get; set; } // FK_RotationMember_Rotation
        public virtual WorkflowNode WorkflowNode { get; set; } // FK_RotationMember_WorkflowNode

        public RotationUser()
        {
            FlagPermission = 0;
        }
    }
}
