using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Workflows", Schema = "public")]
    public class Workflow
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 100)
        public string Description { get; set; } // Description
        public bool IsActive { get; set; } // IsActive
        public long? CreatorId { get; set; } // CreatorId
        public bool IsTemplate { get; set; } // IsTemplate
        public int Type { get; set; } // WfType
        public int TotalUsed { get; set; } // WfType
        public string UserEmail { get; set; } // UserEmail (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<WorkflowNode> WorkflowNodes { get; set; } // WorkflowNode.FK_WorkflowNode_Workflow

        public Workflow()
        {
            WorkflowNodes = new System.Collections.Generic.List<WorkflowNode>();
            Type = 0;
        }
    }
}
