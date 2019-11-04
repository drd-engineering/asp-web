using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
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
        public virtual System.Collections.Generic.ICollection<DocumentElement> DocumentElements { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public virtual System.Collections.Generic.ICollection<RotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_Document
        
        public Company Companies { get; set } //FK to Company

        public DocumentMember DocumentMember { get; set; }

        public Document()
        {
            DocumentMember = new DocumentMember();
            DocumentAnnotates = new System.Collections.Generic.List<DocumentAnnotate>();
            RotationNodeDocs = new System.Collections.Generic.List<RotationNodeDoc>();
        }
    }
}
