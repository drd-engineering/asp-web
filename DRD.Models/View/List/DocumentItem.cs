using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.List
{
    public class DocumentItem
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

        public virtual System.Collections.Generic.ICollection<DocumentUser> DocumentUsers { get; set; } // DocumentUser.FK_DocumentUser_Document
        public virtual System.Collections.Generic.ICollection<DocumentElement> DocumentElements { get; set; } // DocumentElement.FK_DocumentElement_Document
        public DocumentItem()
        {
            DocumentUsers = new System.Collections.Generic.List<DocumentUser>();
        }
    }
}
