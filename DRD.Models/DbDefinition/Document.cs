using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Documents", Schema = "public")]
    public class Document : BaseEntity
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public string Extension { get; set; } // extension file (length: 500)
        public string FileUrl { get; set; } // FileUrl --> file path
        public string FileName { get; set; } // FileName (length: 100)
        public int FileSize { get; set; } // FileSize
        public string LatestVersion { get; set; } // Latest version of document

        public int MaximumPrintPerUser { get; set; }
        public int MaximumDownloadPerUser { get; set; }

        public bool IsCurrent { get; set; }
        public long UploaderId { get; set; }
        public long CompanyId { get; set; } = 0;
        public long RotationId { get; set; }

        public virtual System.Collections.Generic.ICollection<DocumentAnnotation> DocumentAnnotations { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_Document
        public virtual System.Collections.Generic.ICollection<DocumentUser> DocumentUsers { get; set; } // DocumentUser.FK_document
        public virtual System.Collections.Generic.ICollection<DocumentHistory> DocumentHistories { get; set; } // DocumentHistories
        
        // FK
        [ForeignKey("UploaderId")]
        public User Uploader { get; set; } //FK to User

        [ForeignKey("CompanyId")]
        public Company Company { get; set; } //FK to Company

        [ForeignKey("RotationId")]
        public Rotation Rotation { get; set; }

        public Document()
        {
            FileSize = 0;
            MaximumPrintPerUser = 0;
            MaximumDownloadPerUser = 0;
            DocumentAnnotations = new System.Collections.Generic.List<DocumentAnnotation>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
            DocumentHistories = new System.Collections.Generic.List<DocumentHistory>();
        }
    }
}
