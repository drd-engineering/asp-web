using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models.API
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
        public System.DateTime? CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated

        //public StatusCode StatusCode { get; set; }
        public string Note { get; set; }

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationNodeInboxData> RotationNodes { get; set; } // RotationNode.FK_RotationNode_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeDocInboxData> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode

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
            User = new UserInboxData();
            WorkflowNode = new WorkflowNodeInboxData();
        }
        public RotationNodeInboxData(RotationNode rotationNodeDb)
        {
            Id = rotationNodeDb.Id;
            CreatedAt = rotationNodeDb.CreatedAt;
            Status = rotationNodeDb.Status;
            WorkflowNodeId = rotationNodeDb.WorkflowNodeId;
            PrevWorkflowNodeId = rotationNodeDb.PreviousWorkflowNodeId;
            SenderRotationNodeId = rotationNodeDb.SenderRotationNodeId;
            User = new UserInboxData(rotationNodeDb.User);
            WorkflowNode = new WorkflowNodeInboxData(rotationNodeDb.WorkflowNode);
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDocInboxData>();
            foreach(RotationNodeDoc rndDb in rotationNodeDb.RotationNodeDocs)
            {
                RotationNodeDocs.Add(new RotationNodeDocInboxData(rndDb));
            }
        }
    }
}
