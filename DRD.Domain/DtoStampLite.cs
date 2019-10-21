using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoStampLite
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Descr { get; set; } // Descr (length: 1000)
        public string StampFile { get; set; } // StampFile (length: 100)
        public System.DateTime DateCreated; // DateCreated
    }
}
