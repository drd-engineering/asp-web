using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodes", Schema = "public")]
    public class RotationNode : BaseEntity
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long UserId { get; set; } // UserId (Foreign Key)
        public long RotationId { get; set; } // RotationId (Foreign Key)
        public long FirstNodeId { get; set; }
        public long WorkflowNodeId { get; set; } // WorkflowNodeId (Foreign Key)
        public long? PreviousWorkflowNodeId { get; set; } // PrevWorkflowNodeId
        public long? SenderRotationNodeId { get; set; } // SenderRotationNodeId
        public int Status { get; set; } // string

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
       
        // Foreign keys
        public virtual User User { get; set; } // FK_RotationNode_Member

        public virtual Rotation Rotation { get; set; } // FK_RotationNode_Rotation
        public virtual WorkflowNode WorkflowNode { get; set; } // FK_RotationNode_WorkflowNode

        [ForeignKey("SenderRotationNodeId")]
        public virtual RotationNode RotationNode_SenderRotationNodeId { get; set; } // FK_RotationNode_RotationNode

        public RotationNode()
        {
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
        }
    }
}
