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

        public RotationNodeDocInboxData()
        {
            ActionStatus = 0;
            Document = new DocumentInboxData();
        }
        public RotationNodeDocInboxData(RotationNodeDoc rotationNodeDocDb)
        {
            Id = rotationNodeDocDb.Id;
            ActionStatus = rotationNodeDocDb.ActionStatus;
            DocumentId = rotationNodeDocDb.DocumentId;
            Document = new DocumentInboxData(rotationNodeDocDb.Document);
        }
    }
}
