using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodeUpDocs", Schema = "public")]
    public class RotationNodeUpDoc
    {
        public long Id { get; set; } // Id (Primary key)
        public long? DocumentId { get; set; } // DocumentUploadId
        public Document Document { get; set; } // DocumentUploadId

        // Foreign keys
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeUpDoc_RotationNode

        public RotationNodeUpDoc()
        {
        }
    }
}
