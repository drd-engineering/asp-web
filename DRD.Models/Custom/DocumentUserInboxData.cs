namespace DRD.Models
{
    public class DocumentUserInboxData
    {
        public long Id { get; set; } // Id (Primary key)
        public long DocumentId { get; set; }
        public long UserId { get; set; } // MemberId
        public int FlagPermission { get; set; } // FlagPermission
        public int FlagAction { get; set; } // FlagAction

        public string UserName { get; set; }

        // Foreign keys
        public virtual Document Document { get; set; } // FK_DocumentMember_Document
        public virtual User User { get; set; } // FK_DocumentMember_Member

        public DocumentUserInboxData()
        {
            FlagPermission = 0;
            FlagAction = 0;
        }
    }
}
