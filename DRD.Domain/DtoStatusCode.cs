using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoStatusCode
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 2)
        public string Descr { get; set; } // Descr (length: 100)
        public string TextColor { get; set; } // TextColor (length: 20)
        public string BackColor { get; set; } // BackColor (length: 20)
        public string Icon { get; set; } // Icon (length: 100)
    }
}
