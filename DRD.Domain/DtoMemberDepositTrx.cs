using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberDepositTrx
    {
        public long Id { get; set; } // Id (Primary key)
        public string TrxNo { get; set; } // TrxNo (length: 20)
        public System.DateTime TrxDate { get; set; } // TrxDate
        public string TrxType { get; set; } // TrxType (length: 10)
        public long TrxId { get; set; } // TrxId
        public string Descr { get; set; } // Descr (length: 500)
        public long MemberId { get; set; } // MemberId
        public decimal Amount { get; set; } // Amount
        public int DbCr { get; set; } // CrDr
        public System.DateTime DateCreated { get; set; } // TrxDate
    }
}
