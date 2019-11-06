using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Postgres.Models
{
    public class RotationNode
    {
        public long Id { get; set; } // Id (Primary key)
        public long? PrevWorkflowNodeId { get; set; } // PrevWorkflowNodeId
        public long? SenderRotationNodeId { get; set; } // SenderRotationNodeId
        public string Value { get; set; } // Value (length: 20)
        public string Status { get; set; } // string
        public System.DateTime? DateRead { get; set; } // DateRead
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        //public StatusCode StatusCode { get; set; }
        public string Note { get; set; }

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        // public virtual System.Collections.Generic.ICollection<RotationNodeRemark> RotationNodeRemarks { get; set; } // RotationNodeRemark.FK_RotationNodeRemark_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeUpDoc> RotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode


        // Foreign keys
        public virtual Member Member { get; set; } // FK_RotationNode_Member
        public virtual Rotation Rotation { get; set; } // FK_RotationNode_Rotation
        // public virtual WorkflowNode WorkflowNode { get; set; } // FK_RotationNode_WorkflowNode
        // public virtual WorkflowNode PrevWorkflowNode { get; set; } // FK_RotationNode_PrevWorkflowNode
        public virtual RotationNode RotationNode_SenderRotationNodeId { get; set; } // FK_RotationNode_RotationNode

        public RotationNode()
        {
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
            // RotationNodeRemarks = new System.Collections.Generic.List<RotationNodeRemark>();
            RotationNodeUpDocs = new System.Collections.Generic.List<RotationNodeUpDoc>();
        }
    }
}
