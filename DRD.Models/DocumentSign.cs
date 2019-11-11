using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("DocumentSigns", Schema = "public")]
    public class DocumentSign
    {
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 500)
        public string FileName { get; set; } // FileName (length: 100)
        public string FileNameOri { get; set; } // FileNameOri (length: 100)
        public string ExtFile { get; set; } // ExtFile (length: 20)
        public string Version { get; set; } // Version (length: 20)

        public int CxSignature  { get; set; }
        public int CxInitial { get; set; }
        public int CxAnnotate { get; set; } 

        public int RowCount { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
