using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodeDocs", Schema = "public")]
    public class RotationNodeDoc
    {
        public long Id { get; set; } // Id (Primary key)
        public int FlagAction { get; set; } // FlagAction

        public long DocumentId{get; set;}
        public long RotationNodeId{get; set;}
        // Foreign keys
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; } // FK_RotationNodeDoc_Document
        [ForeignKey("RotationNodeId")]
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeDoc_RotationNode1

        public RotationNodeDoc()
        {
            FlagAction = 0;
        }
    }
}
