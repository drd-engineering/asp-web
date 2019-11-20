using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Documents", Schema = "public")]
    public class Document
    {
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 500)
        public string Descr { get; set; } // Descr
        public string FileName { get; set; } // FileName (length: 100)
        public int FileSize { get; set; } // FileSize
        public long? CreatorId { get; set; } // CreatorId
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<DocumentUser> DocumentUsers { get; set; } // DocumentMember.FK_DocumentMember_Document
        public virtual System.Collections.Generic.ICollection<DocumentElement> DocumentElements { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_Document
        
        // FK
        public Company Companies { get; set; } //FK to Company
        [ForeignKey("DocumentId")]
        public DocumentUser DocumentUser { get; set; } // FK to documentuser
        
        public Document()
        {
            DocumentUser = new DocumentUser();
            DocumentElements = new System.Collections.Generic.List<DocumentElement>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
        }
    }
}
