using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{

    public class RotationNodeInboxData
    {
        
        public long Id { get; set; } // Id (Primary key)
        public long UserId { get; set; } // UserId (Foreign Key)
        public long MemberId { get; set; } // MemberId (Foreign Key)
        public long RotationId { get; set; } // RotationId (Foreign Key)
        public long WorkflowNodeId { get; set; } // WorkflowNodeId (Foreign Key)
        public long? PrevWorkflowNodeId { get; set; } // PrevWorkflowNodeId
        public long? SenderRotationNodeId { get; set; } // SenderRotationNodeId
        public string Value { get; set; } // Value (length: 20)
        public int Status { get; set; } // string
        public System.DateTime? DateRead { get; set; } // DateRead
        public System.DateTime CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated

        //public StatusCode StatusCode { get; set; }
        public string Note { get; set; }

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationNodeInboxData> RotationNodes { get; set; } // RotationNode.FK_RotationNode_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeDocInboxData> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeRemark> RotationNodeRemarks { get; set; } // RotationNodeRemark.FK_RotationNodeRemark_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeUpDocInboxData> RotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode

        // Foreign keys
        public virtual UserInboxData User { get; set; } // FK_RotationNode_Member

        public virtual Rotation Rotation { get; set; } // FK_RotationNode_Rotation
        public virtual WorkflowNodeInboxData WorkflowNode { get; set; } // FK_RotationNode_WorkflowNode
        // public virtual WorkflowNode PrevWorkflowNode { get; set; } // FK_RotationNode_PrevWorkflowNode
        [ForeignKey("SenderRotationNodeId")]
        public virtual RotationNode RotationNode_SenderRotationNodeId { get; set; } // FK_RotationNode_RotationNode

        public RotationNodeInboxData()
        {
            RotationNodes = new System.Collections.Generic.List<RotationNodeInboxData>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDocInboxData>();
            RotationNodeRemarks = new System.Collections.Generic.List<RotationNodeRemark>();
            RotationNodeUpDocs = new System.Collections.Generic.List<RotationNodeUpDocInboxData>();
        }
    }
}
