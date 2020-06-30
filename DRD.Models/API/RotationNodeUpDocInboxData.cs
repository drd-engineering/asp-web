using System.ComponentModel.DataAnnotations;

namespace DRD.Models.API
{

    public class RotationNodeUpDocInboxData
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long? DocumentId { get; set; } // DocumentUploadId
        public Document Document { get; set; } // DocumentUploadId
        public long RotationNodeId { get; set; } // RotationNodeId

        // Foreign keys
        public virtual RotationNodeInboxData RotationNode { get; set; } // FK_RotationNodeUpDoc_RotationNode

        public RotationNodeUpDocInboxData()
        {
            RotationNode = new RotationNodeInboxData();
        }
    }
}
