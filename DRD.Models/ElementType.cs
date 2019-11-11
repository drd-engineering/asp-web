using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models
{
    public class ElementType
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 20)
        public string Description { get; set; } // Descr (length: 500)
    }
}
