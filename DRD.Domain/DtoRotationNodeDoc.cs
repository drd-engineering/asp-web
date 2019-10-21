using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoRotationNodeDoc
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationNodeId { get; set; } // RotationNodeId
        public long? DocumentId { get; set; } // DocumentId
        public int FlagAction { get; set; } // FlagAction

        // Foreign keys
        public virtual DtoDocument Document { get; set; } // FK_RotationNodeDoc_Document
        public virtual DtoRotationNode RotationNode { get; set; } // FK_RotationNodeDoc_RotationNode1

        public DtoRotationNodeDoc()
        {
            FlagAction = 0;
        }
    }
}
