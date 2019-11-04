using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoVersioning
    {
        public int Id { get; set; } // Id (Primary key)
        public string PackageName { get; set; } // PackageName (length: 100)
        public int VersionCode { get; set; } // Version (length: 10)
        public string VersionName { get; set; } // Version (length: 50)
        public string Version { get; set; } // Version (length: 10)
    }
}
