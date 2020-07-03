using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{

    public class WorkflowNodeInboxData
    {

        public long Id { get; set; } // Id (Primary key)
        public long WorkflowId { get; set; } // WorkflowId
        public long? UserId { get; set; } // FK to User
        public int SymbolCode { get; set; } // SymbolId
        public string Caption { get; set; } // Caption (length: 100)
        public string Info { get; set; } // Info (length: 1000)
        public string Operator { get; set; } // Operator (length: 10)
        public string Value { get; set; } // Value (length: 20)
        public string PosLeft { get; set; } // PosLeft (length: 10)
        public string PosTop { get; set; } // PosTop (length: 10)
        public string Width { get; set; } // Width (length: 10)
        public string Height { get; set; } // Height (length: 10)
        public string TextColor { get; set; } // TextColor (length: 20)
        public string BackColor { get; set; } // BackColor (length: 20)
        public int Flag { get; set; } // Flag

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<RotationUser> RotationUsers { get; set; } // RotationUser.FK_RotationUser_WorkflowNode
        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_WorkflowNode
        [ForeignKey("WorkflowNodeId")]
        [InverseProperty("WorkflowNodes")]
        public virtual System.Collections.Generic.ICollection<WorkflowNodeLink> WorkflowNodeLinks { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode
        [ForeignKey("WorkflowNodeToId")]
        [InverseProperty("WorkflowNodeTos")]
        public virtual System.Collections.Generic.ICollection<WorkflowNodeLink> WorkflowNodeLinkTos { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_WorkflowNode1

        // Foreign keys
        public virtual Workflow Workflow { get; set; } // FK_WorkflowNode_Workflow

        public WorkflowNodeInboxData()
        {
            Flag = 0;
            RotationUsers = new System.Collections.Generic.List<RotationUser>();
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
            WorkflowNodeLinks = new System.Collections.Generic.List<WorkflowNodeLink>();
            WorkflowNodeLinkTos = new System.Collections.Generic.List<WorkflowNodeLink>();
        }
        public WorkflowNodeInboxData(WorkflowNode wfnDb)
        {
            Id = wfnDb.Id;
            Caption = wfnDb.Caption;
        }
    }
}
