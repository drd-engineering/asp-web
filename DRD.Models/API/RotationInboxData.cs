using System.Collections.Generic;

namespace DRD.Models.API
{
    public class RotationInboxData
    {
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 100)
        public int Status { get; set; } // Status (length: 2)
        public string Remark { get; set; } // Remark
        public long? CreatorId { get; set; } // CreatorId
        public long? CompanyId { get; set; } // filled only if rotation started
        public long FirstNodeId { get; set; }
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public string StatusDescription { get; set; }
        public long RotationNodeId { get; set; }
        public long DefWorkflowNodeId { get; set; }
        public long WorkflowId { get; set; }
        public long? UserId { get; set; }
        public int FlagAction { get; set; }
        public byte SubscriptionType { get; set; }
        public int AccessType { get; set; }
        public long SubscriptionOf { set; get; }
        public string DecissionInfo { get; set; }
        public DocumentInboxData Document { get; set; }

        // Document summaries
        public virtual System.Collections.Generic.ICollection<RotationNodeDocInboxData> SumRotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeUpDoc> SumRotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationUser> RotationUsers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual System.Collections.Generic.ICollection<RotationNodeInboxData> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        // Foreign keys
        public virtual Workflow Workflow { get; set; } // FK_Rotation_Workflow

        public virtual SmallCompanyData CompanyInbox { get; set; } // FK_Rotation_Company
        public ICollection<string> Tags { get; set; } // Tags
        //[ForeignKey("UserId")]
        //public virtual User User { get; set; } // FK_Rotation_Member

        public RotationInboxData()
        {
            SumRotationNodeDocs = new System.Collections.Generic.List<RotationNodeDocInboxData>();
            SumRotationNodeUpDocs = new System.Collections.Generic.List<RotationNodeUpDoc>();

            Document = new DocumentInboxData();

            RotationUsers = new System.Collections.Generic.List<RotationUser>();
            RotationNodes = new System.Collections.Generic.List<RotationNodeInboxData>();
            CompanyInbox = new SmallCompanyData();
            Tags = new List<string>();
        }
    }
}