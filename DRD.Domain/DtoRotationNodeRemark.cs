﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoRotationNodeRemark
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationNodeId { get; set; } // RotationNodeId
        public string Remark { get; set; } // Remark
        public System.DateTime DateStamp { get; set; } // DateStamp

        // Foreign keys
        public virtual DtoRotationNode RotationNode { get; set; } // FK_RotationNodeRemark_RotationNode

        public DtoRotationNodeRemark()
        {
            DateStamp = System.DateTime.Now;
        }
    }
}
