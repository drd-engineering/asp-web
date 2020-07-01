namespace DRD.Models.API
{
    public class RotationNodeDocInboxData
    {
        public long Id { get; set; } // Id (Primary key)
        public int ActionStatus { get; set; } // FlagAction

        public long DocumentId { get; set; }
        public long RotationNodeId { get; set; }
        // Foreign keys
        public virtual DocumentInboxData Document { get; set; } // FK_RotationNodeDoc_Document
        public virtual RotationNodeInboxData RotationNode { get; set; } // FK_RotationNodeDoc_RotationNode1

        public RotationNodeDocInboxData()
        {
            ActionStatus = 0;
            Document = new DocumentInboxData();
            RotationNode = new RotationNodeInboxData();
        }
    }
}
