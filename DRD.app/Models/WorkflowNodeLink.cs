using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.app.Models
{
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
        public virtual WorkflowNode WorkflowNode_WorkflowNodeId { get; set; } // FK_WorkflowNodeLink_WorkflowNode
        // public virtual WorkflowNode WorkflowNode_WorkflowNodeToId { get; set; } // FK_WorkflowNodeLink_WorkflowNode1
    }
}
