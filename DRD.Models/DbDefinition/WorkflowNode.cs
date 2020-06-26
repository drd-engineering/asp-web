using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("WorkflowNodes", Schema = "public")]
    public class WorkflowNode : BaseEntity
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long WorkflowId { get; set; } // WorkflowId
        public int SymbolCode { get; set; } // SymbolId
        public string Caption { get; set; } // Caption (length: 100)
        public string PosLeft { get; set; } // PosLeft (length: 10)
        public string PosTop { get; set; } // PosTop (length: 10)
        public string Width { get; set; } // Width (length: 10)
        public string Height { get; set; } // Height (length: 10)
        public string TextColor { get; set; } // TextColor (length: 20)
        public string BackColor { get; set; } // BackColor (length: 20)

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationUser> RotationUsers { get; set; } // RotationUser.FK_RotationUser_WorkflowNode
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_WorkflowNode
        [ForeignKey("SourceId")]
        [InverseProperty("Source")]
        public virtual System.Collections.Generic.ICollection<WorkflowNodeLink> Sources { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode
        [ForeignKey("TargetId")]
        [InverseProperty("Target")]
        public virtual System.Collections.Generic.ICollection<WorkflowNodeLink> Targets { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode1

        // Foreign keys
        public virtual Workflow Workflow { get; set; } // FK_WorkflowNode_Workflow

        public WorkflowNode()
        {
            RotationUsers = new System.Collections.Generic.List<RotationUser>();
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
            Sources = new System.Collections.Generic.List<WorkflowNodeLink>();
            Targets = new System.Collections.Generic.List<WorkflowNodeLink>();
        }
    }
}
