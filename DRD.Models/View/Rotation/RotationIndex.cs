using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;
using DRD.Models.Custom;

namespace DRD.Models.View
{
    public class RotationIndex
    {
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 2)
        public long WorkflowId { get; set; } // WorkflowId
        public int Status { get; set; } // Status (length: 2)
        public string Remark { get; set; } // Remark
        public long? UserId { get; set; } // userid
        public long? MemberId { get; set; } //memberid
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        public System.DateTime? DateStatus { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public long RotationNodeId { get; set; }
        public string ActivityName { get; set; }
        public string WorkflowName { get; set; }
        public string StatusDescription { get; set; }
        public virtual System.Collections.Generic.ICollection<string> Tags { get; set; } // Calon Tag yang akan dibuat atau di ambil
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation
        public virtual System.Collections.Generic.ICollection<RotationUserItem> RotationUsers { get; set; } // RotationNode.FK_RotationNode_Rotation
        public virtual Member Member { get; set; } // FK_Rotation_Member
        public virtual WorkflowItem Workflow { get; set; } // FK_Rotation_Workflow


        public RotationIndex()
        {
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
            RotationUsers = new System.Collections.Generic.List<RotationUserItem>();
            
        }
    }
    public class RotationUserItem
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationId { get; set; } // RotationId
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public long? UserId { get; set; } // MemberId
        public int FlagPermission { get; set; } // FlagPermission

        public string ActivityName { get; set; }
        public string Picture { get; set; }
        public long? Number { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public RotationUserItem()
        {
            FlagPermission = 0;
        }
    }
}
