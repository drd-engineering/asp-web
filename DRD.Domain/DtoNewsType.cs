using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoNewsType
    {
        public int Id { get; set; } // Id (Primary key)
        public string Descr { get; set; } // Descr (length: 50)
        public string Info { get; set; } // Info (length: 1000)
        public int BitValue { get; set; } // BitValue
        public bool IsChecked { get; set; }

        public DtoNewsType()
        {
            BitValue = 0;
            IsChecked = false;
        }
    }
}
