﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Postgres.Models
{
    public class RotationNodeUpDoc
    {
        public long Id { get; set; } // Id (Primary key)
        public long? DocumentUploadId { get; set; } // DocumentUploadId

        // Foreign keys
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeUpDoc_RotationNode

        public RotationNodeUpDoc()
        {
        }
    }
}