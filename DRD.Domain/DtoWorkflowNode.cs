using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoWorkflowNode
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
        public virtual System.Collections.Generic.ICollection<DtoRotationNode> Rotations { get; set; } // Rotation.FK_Rotation_WorkflowNode
        public virtual System.Collections.Generic.ICollection<DtoWorkflowNodeLink> WorkflowNodeLinks_WorkflowNodeId { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode
        public virtual System.Collections.Generic.ICollection<DtoWorkflowNodeLink> WorkflowNodeLinks_WorkflowNodeToId { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode1

        // Foreign keys
        public virtual DtoMember Member { get; set; } // FK_WorkflowNode_Member
        public virtual DtoSymbol Symbol { get; set; } // FK_WorkflowNode_Symbol
        public virtual DtoWorkflow Workflow { get; set; } // FK_WorkflowNode_Workflow

        public DtoWorkflowNode()
        {
            Flag = 0;
            Rotations = new System.Collections.Generic.List<DtoRotationNode>();
            WorkflowNodeLinks_WorkflowNodeId = new System.Collections.Generic.List<DtoWorkflowNodeLink>();
            WorkflowNodeLinks_WorkflowNodeToId = new System.Collections.Generic.List<DtoWorkflowNodeLink>();
        }
    }
}
