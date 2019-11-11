using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Models.API
{
    public class UploadDocument
    {
        public string Title { get; set; } // Title (length: 500)
        public string Descr { get; set; } // Descr
        public string FielUrl { get; set; } // FileName (length: 100)
        public int FileSize { get; set; } // FileSize
        public string UserId { get; set; } // UserId (length: 50)        
        // Reverse navigation
        public System.Collections.Generic.ICollection<DocumentElement> DocumentElements { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public System.Collections.Generic.ICollection<RotationNodeDoc> RotationNodeDocs { get; set; } // RotationNodeDoc.FK_RotationNodeDoc_Document
    }
}
