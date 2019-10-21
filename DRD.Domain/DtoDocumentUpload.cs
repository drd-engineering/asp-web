using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoDocumentUpload
    {
        public long Id { get; set; } // Id (Primary key)
        public string FileName { get; set; } // FileName (length: 100)
        public string FileNameOri { get; set; } // FileNameOri (length: 100)
        public string ExtFile { get; set; } // ExtFile (length: 20)
        public int FileFlag { get; set; } // FileFlag
        public int FileSize { get; set; } // FileSize
        public long? CreatorId { get; set; } // CreatorId
        public System.DateTime DateCreated { get; set; } // DateCreated

        //// Reverse navigation
        //public virtual System.Collections.Generic.ICollection<RotationNodeUpDoc> RotationNodeUpDocs { get; set; } // RotationNodeUpDoc.FK_RotationNodeUpDoc_DocumentUpload

        public DtoDocumentUpload()
        {
            FileFlag = 0;
            FileSize = 0;
            //RotationNodeUpDocs = new System.Collections.Generic.List<RotationNodeUpDoc>();
        }
    }
}
