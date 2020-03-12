using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    public class RotationItem
    {
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 100)
        public long WorkflowId { get; set; } // WorkflowId
        public int Status { get; set; } // Status (length: 2)
        public string Remark { get; set; } // Remark
        public long UserId { get; set; } // MemberId
        public long? CreatorId { get; set; } // CreatorId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        public System.DateTime? DateStatus { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public string StatusDescr { get; set; }
        public long RotationNodeId { get; set; }
        public long DefWorkflowNodeId { get; set; }
        public int FlagAction { get; set; }
        public string DecissionInfo { get; set; }
        public virtual System.Collections.Generic.ICollection<string> Tags { get; set; } // Calon Tag yang akan dibuat atau di ambil

        // Document summaries
        public virtual System.Collections.Generic.ICollection<TagItem> TagItems { get; set; }
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> SumRotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeUpDoc> SumRotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode


        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationUser> RotationUsers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        // Foreign keys
        public virtual User User { get; set; } // FK_Rotation_Member
        public virtual Workflow Workflow { get; set; } // FK_Rotation_Workflow

        public RotationItem()
        {
            SumRotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
            SumRotationNodeUpDocs = new System.Collections.Generic.List<RotationNodeUpDoc>();

            RotationUsers = new System.Collections.Generic.List<RotationUser>();
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
        }
    }
}
