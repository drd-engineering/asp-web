using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.Custom
{
    public class WorkflowData
    {
        public string Key { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long ProjectId { get; set; }
        public bool IsActive { get; set; }
        public long CreatorId { get; set; }
        public bool IsTemplate { get; set; } // IsTemplate
        public int Type { get; set; } // WfType
        public string UserId { get; set; }

        public bool IsUsed { get; set; }
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated


        public ICollection<WorkflowNodeData> WorkflowNodes { get; set; }
        public ICollection<WorkflowNodeLinkData> WorkflowNodeLinks { get; set; }

        public WorkflowData()
        {
            IsUsed = false;
            Type = 0;
        }
    }
}
