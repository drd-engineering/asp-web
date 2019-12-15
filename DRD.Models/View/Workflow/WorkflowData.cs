using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.Workflow
{
    public class WorkflowData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatorId { get; set; }
        public bool IsTemplate { get; set; } // IsTemplate
        public int WfType { get; set; } // WfType
        public string UserEmail { get; set; }

        public bool IsUsed { get; set; }

        // Reverse navigation
/*        public JsonWorkflowProject Project { get; set; }*/
        public ICollection<WorkflowNodeData> WorkflowNodes { get; set; }
        public ICollection<WorkflowNodeLinkData> WorkflowNodeLinks { get; set; }
    }
}
