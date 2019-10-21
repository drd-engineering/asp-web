using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonWorkflow
    {
        public long Id { get; set; } 
        public string Name { get; set; } 
        public string Descr { get; set; } 
        public long ProjectId { get; set; } 
        public bool IsActive { get; set; }
        public long CreatorId { get; set; }
        public bool IsTemplate { get; set; } // IsTemplate
        public int WfType { get; set; } // WfType
        public string UserId { get; set; }

        public bool IsUsed { get; set; }

        // Reverse navigation
        public JsonWorkflowProject Project { get; set; }
        public ICollection<JsonWorkflowNode> WorkflowNodes { get; set; }
        public ICollection<JsonWorkflowNodeLink> WorkflowNodeLinks { get; set; }

        public JsonWorkflow()
        {
            IsUsed = false;
            WfType = 0;
        }
    }
}
