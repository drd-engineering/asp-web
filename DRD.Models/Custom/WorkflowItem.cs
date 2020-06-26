using System.Collections.Generic;

namespace DRD.Models.Custom
{
    public class WorkflowItem
    {
        public string Key { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatorId { get; set; }
        public bool IsTemplate { get; set; } // IsTemplate

        public bool IsUsed { get; set; }
        public System.DateTime? CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated

        public ICollection<WorkflowNodeItem> WorkflowNodes { get; set; }
        public ICollection<WorkflowNodeLinkItem> WorkflowNodeLinks { get; set; }

        public WorkflowItem()
        {
            IsUsed = false;
        }
    }
}
