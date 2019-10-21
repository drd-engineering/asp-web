using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoVoucher
    {
        public long Id { get; set; } // Id (Primary key)
        public string Number { get; set; } // Number (length: 50)
        public decimal Nominal { get; set; } // Nominal
        public decimal Price { get; set; } // Price
        public int VoucherType { get; set; } // VoucherType
        public long? TrxId { get; set; } // TrxId
        public string TrxType { get; set; } // TrxType (length: 10)
        public long? TrxUserId { get; set; } // TrxUserId
        public System.DateTime? DateUsed { get; set; } // DateUsed
        public System.DateTime DateCreated { get; set; } // DateCreated

        public DtoVoucher()
        {
            VoucherType = 0;
            DateCreated = System.DateTime.Now;
        }
    }
}
