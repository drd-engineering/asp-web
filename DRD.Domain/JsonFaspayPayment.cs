using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonFaspayPayment
    {
        public long Id { get; set; } // Id (Primary key)
        public long PayId { get; set; } // PayId
        public string PayType { get; set; } // PayType (length: 5)
        public long TrxId { get; set; } // TrxId
        public string TrxNo { get; set; } // TrxNo (length: 50)
        public string TrxType { get; set; } // TrxType (length: 5)
        public System.DateTime DateCreated { get; set; } // DateCreated
    }
}
