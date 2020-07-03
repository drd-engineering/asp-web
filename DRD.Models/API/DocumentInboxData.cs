namespace DRD.Models.API
{
    public class DocumentInboxData
    {
        public long Id { get; set; } // Id (Primary key)
        public string Status { get; set; }
        public string Extension { get; set; } // Title (length: 500)
        public string Description { get; set; } // Descr
        public string FileUrl { get; set; } // FileUrl --> file path
        public string FileName { get; set; } // FileName (length: 100)
        public int FileSize { get; set; } // FileSize

        public int MaximumPrintPerUser { get; set; }
        public int MaximumDownloadPerUser { get; set; }
        public int ExpiryDay { get; set; } // Day Counter (Count Down
        // public bool IsCurrent { get; set; }

        public string UserEmail { get; set; } // this existance's questionable
        public bool IsCurrent { get; set; }
        public long CreatorId { get; set; }
        public long CompanyId { get; set; }
        public long RotationId { get; set; }

        public System.DateTime? CreatedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }

        public virtual System.Collections.Generic.ICollection<DocumentAnnotationsInboxData> DocumentAnnotations { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public virtual System.Collections.Generic.ICollection<DocumentUserInboxData> DocumentUsers { get; set; }
        public DocumentUserInboxData DocumentUser { get; set; }
        
        public DocumentInboxData()
        {
            DocumentAnnotations = new System.Collections.Generic.List<DocumentAnnotationsInboxData>();
            DocumentUsers = new System.Collections.Generic.List<DocumentUserInboxData>();
        }
        public DocumentInboxData(Document documentDb)
        {
            Id = documentDb.Id;
            Extension = documentDb.Extension;
            FileUrl = documentDb.FileUrl;
            FileName = documentDb.FileName;
            FileSize = documentDb.FileSize;
            IsCurrent = documentDb.IsCurrent;
            CreatedAt = documentDb.CreatedAt;
            UpdatedAt = documentDb.CreatedAt;
            DocumentUsers = new System.Collections.Generic.List<DocumentUserInboxData>();
            foreach (var documentUserDb in documentDb.DocumentUsers)
            {
                DocumentUsers.Add(new DocumentUserInboxData(documentUserDb));
            }
            DocumentAnnotations = new System.Collections.Generic.List<DocumentAnnotationsInboxData>();
            foreach (var documentAnnotationDb in documentDb.DocumentAnnotations)
            {
                DocumentAnnotations.Add(new DocumentAnnotationsInboxData(documentAnnotationDb));
            }
        }
    }
}
