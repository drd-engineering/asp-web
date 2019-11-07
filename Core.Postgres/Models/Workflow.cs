using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Postgres.Models
{
    public class Workflow
    {
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 100)
        public string Description { get; set; } // Description
        public bool IsActive { get; set; } // IsActive
        public long? CreatorId { get; set; } // CreatorId
        public bool IsTemplate { get; set; } // IsTemplate
        public int WfType { get; set; } // WfType
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<WorkflowNode> WorkflowNodes { get; set; } // WorkflowNode.FK_WorkflowNode_Workflow

        public Workflow()
        {
            WorkflowNodes = new System.Collections.Generic.List<WorkflowNode>();
            WfType = 0;
        }
    }
}
