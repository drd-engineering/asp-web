using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoVoucherGenerator
    {
        public long Id { get; set; } // Id (Primary key)
        public string Number { get; set; } // Number (length: 20) 
        public decimal Nominal { get; set; } // Nominal
        public decimal Price { get; set; } // Price
        public int Quantity { get; set; } // Quantity
        public int VoucherType { get; set; } // VoucherType
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated

        public DtoVoucherGenerator()
        {
            Nominal = 0m;
            Price = 0m;
            Quantity = 0;
            VoucherType = 0;
        }
    }
}
