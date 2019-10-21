using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoDrDrive
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Descr { get; set; } // Descr
        public string FileName { get; set; } // FileName (length: 100)
        public string FileNameOri { get; set; } // FileNameOri (length: 100)
        public string ExtFile { get; set; } // ExtFile (length: 20)
        public int FileFlag { get; set; } // FileFlag
        public long FileSize { get; set; } // FileSize
        public int CxDownload { get; set; } // CxDownload
        public long MemberFolderId { get; set; } // MemberFolderId
        public long? MemberId { get; set; } // MemberId
        public long? DocumentId { get; set; } // DocumentId
        public long? DocumentUploadId { get; set; } // DocumentUploadId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public DtoDrDrive()
        {
            FileFlag = 0;
            FileSize = 0;
            CxDownload = 0;
            MemberFolderId = 0;
        }
    }
}
