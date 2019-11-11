using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("RotationNodeRemarks", Schema = "public")]
    public class RotationNodeRemark
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationNodeId { get; set; } // RotationNodeId
        public string Remark { get; set; } // Remark
        public System.DateTime DateStamp { get; set; } // DateStamp

        // Foreign keys
        public virtual RotationNode RotationNode { get; set; } // FK_RotationNodeRemark_RotationNode

        public RotationNodeRemark()
        {
            DateStamp = System.DateTime.Now;
        }
    }
}
