using System.Collections.Generic;

namespace DRD.Models.API
{
    public class RotationListItem
    {
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 2) --> current Activity
        public long WorkflowId { get; set; } // WorkflowId
        public string WorkflowName { get; set; }
        public int Status { get; set; } // Status (length: 2)
        public string StatusDescription { get; set; }
        public System.DateTime? CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated
        public System.DateTime? StartedAt { get; set; } // DateStarted

        public long RotationNodeId { get; set; }
        public string ActivityName { get; set; }
        public long? CompanyId { get; set; }
        public SmallCompanyData CompanyRotation { get; set; }

    }
}
