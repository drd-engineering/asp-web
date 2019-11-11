using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodeDocs", Schema = "public")]
    public class RotationNodeDoc
    {
        public long Id { get; set; } // Id (Primary key)
        public int FlagAction { get; set; } // FlagAction

        // Foreign keys
        public virtual Document Document { get; set; } // FK_RotationNodeDoc_Document
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeDoc_RotationNode1

        public RotationNodeDoc()
        {
            FlagAction = 0;
        }
    }
}
