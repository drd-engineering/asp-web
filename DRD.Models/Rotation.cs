﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Rotations", Schema = "public")]
    public class Rotation
    {
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 100)
        public int Status { get; set; } // Status (length: 2)
        public string Remark { get; set; } // Remark
        public long? CreatorId { get; set; } // CreatorId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public string StatusDescription { get; set; }
        public long RotationNodeId { get; set; }
        public long DefWorkflowNodeId { get; set; }
        public long WorkflowId { get; set; }
        public long? MemberId { get; set; }
        public long? UserId { get; set; }
        public int FlagAction { get; set; }
        public byte SubscriptionType { get; set; }
        public long SubscriptionOf { set; get; }
        public string DecissionInfo { get; set; }

        // Document summaries
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> SumRotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_RotationNode
        public virtual System.Collections.Generic.ICollection<RotationNodeUpDoc> SumRotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_RotationNode

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationUser> RotationUsers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        // Foreign keys
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; } // FK_Rotation_Member company
        [ForeignKey("UserId")]
        public virtual User User { get; set; } // FK_Rotation_User personal
        [ForeignKey("WorkflowId")]
        public virtual Workflow Workflow { get; set; } // FK_Rotation_Workflow

        public Rotation()
        {
            SumRotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
            SumRotationNodeUpDocs = new System.Collections.Generic.List<RotationNodeUpDoc>();

            RotationUsers = new System.Collections.Generic.List<RotationUser>();
            RotationNodes = new System.Collections.Generic.List<RotationNode>();

            Member = new Member();
            User = new User();
            Workflow = new Workflow();
        }
    }
}
