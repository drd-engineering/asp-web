using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models
{
    public class AnnotateType
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 20)
        public string Descr { get; set; } // Descr (length: 500)
    }
}
