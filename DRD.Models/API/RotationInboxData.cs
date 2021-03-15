using System.Collections.Generic;

namespace DRD.Models.API
{
    public class RotationInboxData
    {
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Subject (length: 100)
        public int Status { get; set; } // Status (length: 2)
        public int RotationStatus { get; set; } // 
        public long? CreatorId { get; set; } // CreatorId
        public long? CompanyId { get; set; } // filled only if rotation started
        public string EncryptCID { get; set; } // filled only if rotation started
        public long FirstNodeId { get; set; }
        public System.DateTime? CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated
        public System.DateTime? StartedAt { get; set; } // DateStarted
        public long RotationNodeId { get; set; }
        public long CurrentActivity { get; set; }
        public long WorkflowId { get; set; }
        public long? UserId { get; set; }
        public int ActionStatus { get; set; }
        public int AccessType { get; set; }
        public int DocumentActionPermissionType { get; set; }
        public DocumentInboxData Document { get; set; }

        // Document summaries
        public virtual System.Collections.Generic.ICollection<RotationNodeDocInboxData> SumRotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationUser> RotationUsers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual System.Collections.Generic.ICollection<RotationNodeInboxData> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        // Foreign keys
        public virtual Workflow Workflow { get; set; } // FK_Rotation_Workflow
        public virtual SmallCompanyData CompanyInbox { get; set; } // FK_Rotation_Company
        public ICollection<string> Tags { get; set; } // Tags
        public RotationInboxData()
        {
            SumRotationNodeDocs = new System.Collections.Generic.List<RotationNodeDocInboxData>();

            Document = new DocumentInboxData();

            RotationUsers = new List<RotationUser>();
            RotationNodes = new List<RotationNodeInboxData>();
            CompanyInbox = new SmallCompanyData();
            Tags = new List<string>();
        }
    }
}