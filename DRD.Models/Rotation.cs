using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Rotations", Schema = "public")]
    public class Rotation
    {
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 100)
        public string Status { get; set; } // Status (length: 2)
        public string Remark { get; set; } // Remark
        public long? CreatorId { get; set; } // CreatorId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public string StatusDescr { get; set; }
        public long RotationNodeId { get; set; }
        public long DefWorkflowNodeId { get; set; }
        public int FlagAction { get; set; }
        public string DecissionInfo { get; set; }

        // Document summaries
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> SumRotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeUpDoc> SumRotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode


        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationMember> RotationMembers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        // Foreign keys
        public virtual Member Member { get; set; } // FK_Rotation_Member
        public virtual Workflow Workflow { get; set; } // FK_Rotation_Workflow

        public Rotation()
        {
            SumRotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
            SumRotationNodeUpDocs = new System.Collections.Generic.List<RotationNodeUpDoc>();

            RotationMembers = new System.Collections.Generic.List<RotationMember>();
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
        }
    }
}
