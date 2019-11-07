using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.app.Models
{
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
