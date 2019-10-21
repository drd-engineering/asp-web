using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoRotationNode
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationId { get; set; } // RotationId
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public long? PrevWorkflowNodeId { get; set; } // PrevWorkflowNodeId
        public long? SenderRotationNodeId { get; set; } // SenderRotationNodeId
        public long MemberId { get; set; } // MemberId
        public string Value { get; set; } // Value (length: 20)
        public string Status { get; set; } // string
        public System.DateTime? DateRead { get; set; } // DateRead
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public DtoStatusCode StatusCode { get; set; }
        public string Note { get; set; }

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<DtoRotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_RotationNode
        public virtual System.Collections.Generic.ICollection<DtoRotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        public virtual System.Collections.Generic.ICollection<DtoRotationNodeRemark> RotationNodeRemarks { get; set; } // RotationNodeRemark.FK_RotationNodeRemark_RotationNode
        public virtual System.Collections.Generic.ICollection<DtoRotationNodeUpDoc> RotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode


        // Foreign keys
        public virtual DtoMember Member { get; set; } // FK_RotationNode_Member
        public virtual DtoRotation Rotation { get; set; } // FK_RotationNode_Rotation
        public virtual DtoWorkflowNode WorkflowNode { get; set; } // FK_RotationNode_WorkflowNode
        public virtual DtoWorkflowNode PrevWorkflowNode { get; set; } // FK_RotationNode_PrevWorkflowNode
        public virtual DtoRotationNode RotationNode_SenderRotationNodeId { get; set; } // FK_RotationNode_RotationNode

        public DtoRotationNode()
        {
            RotationNodes = new System.Collections.Generic.List<DtoRotationNode>();
            RotationNodeDocs = new System.Collections.Generic.List<DtoRotationNodeDoc>();
            RotationNodeRemarks = new System.Collections.Generic.List<DtoRotationNodeRemark>();
            RotationNodeUpDocs = new System.Collections.Generic.List<DtoRotationNodeUpDoc>();
        }
    }
}
