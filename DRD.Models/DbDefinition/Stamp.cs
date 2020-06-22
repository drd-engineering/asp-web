using System.ComponentModel.DataAnnotations;

namespace DRD.Models
{
    public class Stamp
    {
        public string Key { get; set; }
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long CompanyId { get; set; } // CompanyId
        public string Descr { get; set; } // Descr (length: 1000)
        public string StampFile { get; set; } // StampFile (length: 100)
        public long? CreatorId { get; set; } // CreatorId
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
    }
}
