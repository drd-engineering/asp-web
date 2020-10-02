namespace DRD.Models.View
{
    public class DocumentItem
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string EncryptedId { get; set; } //generated encryptedid
        public string Extension { get; set; } // Title (length: 500)
        public string FileName { get; set; } // FileName (length: 100)
        public string FileNameOri { get; set; } // FileNameOri (length: 100)
        public int FileSize { get; set; } // FileSize
        public int MaxPrint { get; set; } // MaxPrint
        public int MaxDownload { get; set; } // MaxDownload
        public string LatestVersion { get; set; } // Version (length: 20)
        public long? CreatorId { get; set; } // CreatorId
        public virtual System.Collections.Generic.ICollection<DocumentUser> DocumentUsers { get; set; } // DocumentUser.FK_DocumentUser_Document
        public virtual System.Collections.Generic.ICollection<DocumentAnnotation> DocumentElements { get; set; } // DocumentElement.FK_DocumentElement_Document
        public DocumentItem()
        {
            DocumentUsers = new System.Collections.Generic.List<DocumentUser>();
        }
    }
}
