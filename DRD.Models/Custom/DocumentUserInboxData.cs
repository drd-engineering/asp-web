namespace DRD.Models
{
    public class DocumentUserInboxData
    {
        public long Id { get; set; } // Id (Primary key)
        public long DocumentId { get; set; }
        public long UserId { get; set; } // MemberId
        public int ActionPermission { get; set; } // FlagPermission
        public int ActionStatus { get; set; } // FlagAction
        public int PrintCount { get; set; } //Counter for user print call
        public int DownloadCount { get; set; } //Counter for user Download call

        // Foreign keys
        public virtual Document Document { get; set; } // FK_DocumentMember_Document
        public virtual User User { get; set; } // FK_DocumentMember_Member

        public DocumentUserInboxData()
        {
            ActionPermission = 0;
            ActionStatus = 0;
        }
    }
}
