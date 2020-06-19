using System.Collections.Generic;

namespace DRD.Models.API
{
    public class RotationData
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 2) --> current Activity
        public long WorkflowId { get; set; } // WorkflowId
        public int Status { get; set; } // Status (length: 2)
        public long? UserId { get; set; } // userid
        public long? MemberId { get; set; } //memberid
        public long FirstNodeId { get; set; }
        public System.DateTime CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated
        public System.DateTime? DateStatus { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public long RotationNodeId { get; set; }
        public string ActivityName { get; set; }
        public string WorkflowName { get; set; }
        public string StatusDescription { get; set; }
        public long? CompanyId { get; set; }
        public SmallCompanyData CompanyRotation { get; set; }

        public virtual ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        public RotationData()
        {
            RotationNodes = new List<RotationNode>();
            
        }
    }
}
