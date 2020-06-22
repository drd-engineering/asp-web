using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodeRemarks", Schema = "public")]
    public class RotationNodeRemark
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public string Remark { get; set; } // Remark
        public System.DateTime DateStamp { get; set; } // DateStamp
        public long RotationNodeId { get; set; } // RotationNodeId

        // Foreign keys
        [ForeignKey("RotationNodeId")]
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeRemark_RotationNode

        public RotationNodeRemark()
        {
            DateStamp = System.DateTime.Now;
            RotationNode = new RotationNode();
        }
    }
}
