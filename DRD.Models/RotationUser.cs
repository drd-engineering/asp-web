using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationMembers", Schema = "public")]
    public class RotationUser
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public bool isStartPerson { get; set; } // flag is he starting node person
        public int FlagPermission { get; set; } // FlagPermission
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public string ActivityName { get; set; }
        public string Picture { get; set; }
        public long? UserId { get; set; }
        public long RotationId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // Foreign keys
        [ForeignKey("UserId")]
        public virtual User User { get; set; } // FK_RotationMember_Member
        [ForeignKey("RotationId")]
        public virtual Rotation Rotation { get; set; } // FK_RotationMember_Rotation
        [ForeignKey("WorkflowNodeId")]
        public virtual WorkflowNode WorkflowNode { get; set; } // FK_RotationMember_WorkflowNode

        public RotationUser()
        {
            FlagPermission = 0;

        }
    }
}
