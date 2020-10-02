using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("DocumentHistories", Schema = "public")]
    public class DocumentHistory : BaseEntity
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public string Version { get; set; } // Version (length: 20) version will define where the document stored, so the version will not always changed but the history will always updated
        public bool Latest { get; set; } // Showing the history of document is the latest version
        public string Details { get; set; } // Details about What change happen 
        public long DocumentId { get; set; } // Document Version
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
    }
}
