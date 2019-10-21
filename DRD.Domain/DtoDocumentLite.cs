using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoDocumentLite
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 500)
        public string FileName { get; set; } // FileName (length: 100)
        public string FileNameOri { get; set; } // FileNameOri (length: 100)
        public string ExtFile { get; set; } // ExtFile (length: 20)
        public int FileFlag { get; set; } // FileFlag
        public int FileSize { get; set; } // FileSize
        public int MaxPrint { get; set; } // MaxPrint
        public int MaxDownload { get; set; } // MaxDownload
        public int ExpiryDay { get; set; } // ExpiryDay
        public string Version { get; set; } // Version (length: 20)
        public long? CreatorId { get; set; } // CreatorId
        public long CompanyId { get; set; } // CompanyId
        public string Descr { get; set; } // Descr
        public System.DateTime DateCreated { get; set; } // DateCreated

        public virtual System.Collections.Generic.ICollection<DtoDocumentMember> DocumentMembers { get; set; } // DocumentMember.FK_DocumentMember_Document
        public virtual System.Collections.Generic.ICollection<DtoDocumentAnnotate> DocumentAnnotates { get; set; } // DocumentAnnotate.FK_DocumentAnnotate_Document
        public DtoDocumentLite()
        {
            DocumentMembers = new System.Collections.Generic.List<DtoDocumentMember>();
        }
    }
}
