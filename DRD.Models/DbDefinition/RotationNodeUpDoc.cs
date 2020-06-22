using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodeUpDocs", Schema = "public")]
    public class RotationNodeUpDoc
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long? DocumentId { get; set; } // DocumentUploadId
        public Document Document { get; set; } // DocumentUploadId
        public long RotationNodeId { get; set; } // RotationNodeId
        public long RotationId { get; set; }
        // Foreign keys
        [ForeignKey("RotationNodeId")]
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeUpDoc_RotationNode
        [ForeignKey("RotationId")]
        public virtual Rotation Rotation { get; set; }

        public RotationNodeUpDoc()
        {
            RotationNode = new RotationNode();
        }
    }
}
