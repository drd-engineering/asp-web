using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DRD.Models
{
    [Table("WorkflowNodeLinks", Schema = "public")]
    public class WorkflowNodeLink
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long SourceId { get; set; } // WorkflowNodeId
        public long TargetId { get; set; } // WorkflowNodeToId
        public long FirstNodeId { get; set; } // WorkflowNodeToId
        public int SymbolCode { get; set; } // SymbolId

        // Foreign keys
        public virtual WorkflowNode FirstNode { get; set; } // FK_WorkflowNodeLink_WorkflowNode
        public virtual WorkflowNode Source { get; set; } // FK_WorkflowNodeLink_WorkflowNode
        public virtual WorkflowNode Target { get; set; } // FK_WorkflowNodeLink_WorkflowNode1
    }
}