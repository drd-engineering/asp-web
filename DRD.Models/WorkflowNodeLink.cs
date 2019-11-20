using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace DRD.Models
{
    [Table("WorkflowNodeLinks", Schema = "public")]
    public class WorkflowNodeLink
    {
        public long Id { get; set; } // Id (Primary key)
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public long WorkflowNodeToId { get; set; } // WorkflowNodeToId
        public string Caption { get; set; } // Caption (length: 100)
        public string Operator { get; set; } // Operator (length: 10)
        public string Value { get; set; } // Value (length: 20)
        public int SymbolId { get; set; } // SymbolId

        // Foreign keys
        public virtual Symbol Symbol { get; set; } // FK_WorkflowNodeLink_Symbol
        public virtual WorkflowNode WorkflowNodes { get; set; } // FK_WorkflowNodeLink_WorkflowNode
        public virtual WorkflowNode WorkflowNodeTos { get; set; } // FK_WorkflowNodeLink_WorkflowNode1
    }
}