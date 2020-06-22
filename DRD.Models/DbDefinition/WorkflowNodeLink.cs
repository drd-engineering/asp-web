using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DRD.Models
{
    [Table("WorkflowNodeLinks", Schema = "public")]
    public class WorkflowNodeLink
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public long WorkflowNodeToId { get; set; } // WorkflowNodeToId
        public long FirstNodeId { get; set; } // WorkflowNodeToId
        public long EndNodeId { get; set; } // WorkflowNodeToId
        public string Caption { get; set; } // Caption (length: 100)
        public string Operator { get; set; } // Operator (length: 10)
        public string Value { get; set; } // Value (length: 20)
        public int SymbolCode { get; set; } // SymbolId

        // Foreign keys
        public virtual WorkflowNode FirstNode { get; set; } // FK_WorkflowNodeLink_WorkflowNode
        public virtual WorkflowNode EndNode { get; set; } // FK_WorkflowNodeLink_WorkflowNode
        public virtual WorkflowNode WorkflowNode { get; set; } // FK_WorkflowNodeLink_WorkflowNode
        public virtual WorkflowNode WorkflowNodeTo { get; set; } // FK_WorkflowNodeLink_WorkflowNode1
    }
}