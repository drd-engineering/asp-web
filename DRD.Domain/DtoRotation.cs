using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoRotation
    {
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 100)
        public long WorkflowId { get; set; } // WorkflowId
        public string Status { get; set; } // Status (length: 2)
        public string Remark { get; set; } // Remark
        public long MemberId { get; set; } // MemberId
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

        // Document summaries
        public virtual System.Collections.Generic.ICollection<DtoRotationNodeDoc> SumRotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        public virtual System.Collections.Generic.ICollection<DtoRotationNodeUpDoc> SumRotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode


        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<DtoRotationMember> RotationMembers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual System.Collections.Generic.ICollection<DtoRotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        // Foreign keys
        public virtual DtoMember Member { get; set; } // FK_Rotation_Member
        public virtual DtoWorkflow Workflow { get; set; } // FK_Rotation_Workflow

        public DtoRotation()
        {
            SumRotationNodeDocs = new System.Collections.Generic.List<DtoRotationNodeDoc>();
            SumRotationNodeUpDocs = new System.Collections.Generic.List<DtoRotationNodeUpDoc>();

            RotationMembers = new System.Collections.Generic.List<DtoRotationMember>();
            RotationNodes = new System.Collections.Generic.List<DtoRotationNode>();
        }
    }
}
