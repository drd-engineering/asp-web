using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Versionings", Schema = "public")]
    public class Versioning
    {
        public int Id { get; set; } // Id (Primary key)
        public string PackageName { get; set; } // PackageName (length: 100)
        public int VersionCode { get; set; } // Version (length: 10)
        public string VersionName { get; set; } // Version (length: 50)
        public string Version { get; set; } // Version (length: 10)
    }
}
