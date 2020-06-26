using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Workflows", Schema = "public")]
    public class Workflow : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 100)
        public string Description { get; set; } // Description
        public bool IsActive { get; set; } // IsActive
        public long? CreatorId { get; set; } // CreatorId
        public bool IsTemplate { get; set; } // IsTemplate
        public int TotalUsed { get; set; } // WfType
        
        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<WorkflowNode> WorkflowNodes { get; set; } // WorkflowNode.FK_WorkflowNode_Workflow

        public Workflow()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: ConstantModel.MINIMUM_VALUE_ID, maximumValue: ConstantModel.MAXIMUM_VALUE_ID); 
            WorkflowNodes = new System.Collections.Generic.List<WorkflowNode>();
        }
    }
}
