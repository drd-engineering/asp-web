using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationUsers", Schema = "public")]
    public class RotationUser : BaseEntity
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public bool IsStartPerson { get; set; } // flag is he starting node person
        public int ActionPermission { get; set; } // FlagPermission
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public long? UserId { get; set; }
        public long RotationId { get; set; }

        // Foreign keys
        [ForeignKey("UserId")]
        public virtual User User { get; set; } // FK_RotationMember_Member
        [ForeignKey("RotationId")]
        public virtual Rotation Rotation { get; set; } // FK_RotationMember_Rotation
        [ForeignKey("WorkflowNodeId")]
        public virtual WorkflowNode WorkflowNode { get; set; } // FK_RotationMember_WorkflowNode

        public RotationUser()
        {
            ActionPermission = 0;
        }
    }
}
