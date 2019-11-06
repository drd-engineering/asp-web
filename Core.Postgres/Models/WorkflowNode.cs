using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Postgres.Models
{
    public class WorkflowNode
    {
        public long Id { get; set; } // Id (Primary key)
        public long WorkflowId { get; set; } // WorkflowId
        public long? MemberId { get; set; } // MemberId
        public int SymbolId { get; set; } // SymbolId
        public string Caption { get; set; } // Caption (length: 100)
        public string Info { get; set; } // Info (length: 1000)
        public string Operator { get; set; } // Operator (length: 10)
        public string Value { get; set; } // Value (length: 20)
        public string PosLeft { get; set; } // PosLeft (length: 10)
        public string PosTop { get; set; } // PosTop (length: 10)
        public string Width { get; set; } // Width (length: 10)
        public string Height { get; set; } // Height (length: 10)
        public string TextColor { get; set; } // TextColor (length: 20)
        public string BackColor { get; set; } // BackColor (length: 20)
        public int Flag { get; set; } // Flag

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationNode> Rotations { get; set; } // Rotation.FK_Rotation_WorkflowNode
        // public virtual System.Collections.Generic.ICollection<WorkflowNodeLink> WorkflowNodeLinks_WorkflowNodeId { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode
        // public virtual System.Collections.Generic.ICollection<WorkflowNodeLink> WorkflowNodeLinks_WorkflowNodeToId { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode1

        // Foreign keys
        public virtual Member Member { get; set; } // FK_WorkflowNode_Member
        public virtual Symbol Symbol { get; set; } // FK_WorkflowNode_Symbol
        public virtual Workflow Workflow { get; set; } // FK_WorkflowNode_Workflow

        public WorkflowNode()
        {
            Flag = 0;
            Rotations = new System.Collections.Generic.List<RotationNode>();
            // WorkflowNodeLinks_WorkflowNodeId = new System.Collections.Generic.List<WorkflowNodeLink>();
            // WorkflowNodeLinks_WorkflowNodeToId = new System.Collections.Generic.List<WorkflowNodeLink>();
        }
    }
}
