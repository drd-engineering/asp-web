using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Documents", Schema = "public")]
    public class Document
    {
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 500)
        public string Description { get; set; } // Descr
        public string FileUrl { get; set; } // FileName (length: 100)
        public string FileName { get; set; } // FileName (length: 100)
        public int FileSize { get; set; } // FileSize

        public int MaxPrintPerActivity { get; set; }
        public int MaxDownloadPerActivity { get; set; }
        public int ExpiryDay { get; set; } // Day Counter (Count Down


        public long CreatorId { get; set; } // CreatorId
        public string UserEmail { get; set; }
        public long CompanyId { get; set; }

        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        

        public virtual System.Collections.Generic.ICollection<DocumentElement> DocumentElements { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_Document

        // FK
        [ForeignKey("CompanyId")]
        public Company Companies { get; set; } //FK to Company

        [ForeignKey("CreatorId")]
        public User User { get; set; } //FK to User


        public Document()
        {
            //DocumentUser = new DocumentUser();
            DocumentElements = new System.Collections.Generic.List<DocumentElement>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
        }
    }
}
