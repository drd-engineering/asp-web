using DRD.Models.API;
using DRD.Models.Custom;
using System.Collections.Generic;

namespace DRD.Models.View
{
    public class RotationIndex
    {
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 2)
        public long WorkflowId { get; set; } // WorkflowId
        public int Status { get; set; } // Status (length: 2)
        public string Remark { get; set; } // Remark
        public long? UserId { get; set; } // userid
        public long? MemberId { get; set; } //memberid
        public long? CompanyId { get; set; }
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        public System.DateTime? DateStatus { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public long RotationNodeId { get; set; }
        public string ActivityName { get; set; }
        public string WorkflowName { get; set; }
        public string StatusDescription { get; set; }
        public virtual ICollection<string> Tags { get; set; } // Calon Tag yang akan dibuat atau di ambil
        public virtual ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation
        public virtual ICollection<RotationUserItem> RotationUsers { get; set; } // RotationNode.FK_RotationNode_Rotation
        public virtual Member Member { get; set; } // FK_Rotation_Member
        public virtual WorkflowItem Workflow { get; set; } // FK_Rotation_Workflow
        public SmallCompanyData CompanyRotation { get; set; }

        public RotationIndex()
        {
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
            RotationUsers = new System.Collections.Generic.List<RotationUserItem>();

        }
    }
}
