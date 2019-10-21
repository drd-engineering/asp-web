using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoRotationNodeUpDoc
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationNodeId { get; set; } // RotationNodeId
        public long? DocumentUploadId { get; set; } // DocumentUploadId

        // Foreign keys
        public virtual DtoDocumentUpload DocumentUpload { get; set; } // FK_RotationNodeUpDoc_DocumentUpload
        public virtual DtoRotationNode RotationNode { get; set; } // FK_RotationNodeUpDoc_RotationNode

        public DtoRotationNodeUpDoc()
        {
        }
    }
}
