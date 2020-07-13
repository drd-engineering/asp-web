using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Rotations", Schema = "public")]
    public class Rotation : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Subject (length: 100)
        public int Status { get; set; } // Status (length: 2)
        public string Description { get; set; } // Remark
        public long? CreatorId { get; set; } // CreatorId
        public long? FirstNodeId { get; set; } // CreatorId
        public long? CompanyId { get; set; } // filled only if rotation started
        public System.DateTime? StartedAt { get; set; } // DateStarted
        public long WorkflowId { get; set; }
        public long? UserId { get; set; }
        public byte SubscriptionType { get; set; }
        public bool IsActive { get; set; }

        public virtual System.Collections.Generic.ICollection<TagItem> TagItems { get; set; }
        // Document summaries
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> SumRotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
       
        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationUser> RotationUsers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        // Foreign keys
        [ForeignKey("WorkflowId")]
        public virtual Workflow Workflow { get; set; } // FK_Rotation_Workflow

        public Rotation()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
            SumRotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();

            IsActive = true;

            RotationUsers = new System.Collections.Generic.List<RotationUser>();
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
        }
    }
}
