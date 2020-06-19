using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodeDocs", Schema = "public")]
    public class RotationNodeDoc
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public int FlagAction { get; set; } // FlagAction
        public long RotationId { get; set; }
        public long DocumentId{get; set;}
        public long RotationNodeId{get; set;}
        // Foreign keys
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; } // FK_RotationNodeDoc_Document
        [ForeignKey("RotationNodeId")]
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeDoc_RotationNode1
        [ForeignKey("RotationId")]
        public virtual Rotation Rotation { get; set; }

        public RotationNodeDoc()
        {
            FlagAction = 0;
        }
    }
}
