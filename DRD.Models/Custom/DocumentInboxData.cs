using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{

    public class DocumentInboxData
    {
        
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 500)
        public string Description { get; set; } // Descr
        public string FileUrl { get; set; } // FileUrl --> file path
        public string FileName { get; set; } // FileName (length: 100)
        public int FileSize { get; set; } // FileSize

        public int MaxPrintPerActivity { get; set; }
        public int MaxDownloadPerActivity { get; set; }
        public int ExpiryDay { get; set; } // Day Counter (Count Down
        // public bool IsCurrent { get; set; }

        public string UserEmail { get; set; } // this existance's questionable
        public bool IsCurrent { get; set; }
        public long CreatorId { get; set; }
        public long CompanyId { get; set; }
        public long RotationId { get; set; }

        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }

        public virtual System.Collections.Generic.ICollection<DocumentElementInboxData> DocumentElements { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public virtual System.Collections.Generic.ICollection<RotationNodeDocInboxData> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_Document
        public virtual System.Collections.Generic.ICollection<DocumentUserInboxData> DocumentUsers { get; set; }
        public DocumentUserInboxData DocumentUser { get; set; }
        public Rotation Rotations { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_Document

        // FK
        public Company Company { get; set; } //FK to Company
        public User User { get; set; } //FK to User
        public Rotation Rotation { get; set; }

        public DocumentInboxData()
        {
            //DocumentUser = new DocumentUser();
            DocumentElements = new System.Collections.Generic.List<DocumentElementInboxData>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDocInboxData>();
        }
    }
}
