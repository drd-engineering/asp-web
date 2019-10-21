using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoDrDriveType
    {
        public int Id { get; set; } // Id (Primary key)
        public long Size { get; set; } // Size
        public decimal Price { get; set; } // Price
        public int ExpiryDay { get; set; } // ExpiryDay

        public string StrSize { get; set; } // Size

        public DtoDrDriveType()
        {
            Price = 0m;
            ExpiryDay = 0;
        }
    }
}
