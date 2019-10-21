using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberTopupPayment
    {
        public long Id { get; set; } // Id (Primary key)
        public string PaymentNo { get; set; } // PaymentNo (length: 20)
        public System.DateTime PaymentDate { get; set; } // PaymentDate
        public long TopupDepositId { get; set; } // TopupDepositId
        public decimal Amount { get; set; } // Amount
        public int CompanyBankId { get; set; } // CompanyBankId
        public long? MemberAccountId { get; set; } // MemberAccountId
        public string PaymentStatus { get; set; } // PaymentStatus (length: 2)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string PaymentStatusDescr { get; set; }
        public string VoucherNo { get; set; }
        public string KeyId { get; set; }

        // Foreign keys
        public DtoCompanyBank CompanyBank { get; set; } // FK_MemberTopupPayment_CompanyBank
        public DtoMemberAccount MemberAccount { get; set; } // FK_MemberTopupPayment_MemberAccount
        public DtoMemberTopupDeposit MemberTopupDeposit { get; set; } // FK_MemberTopupPayment_MemberTopupDeposit

        public DtoMemberTopupPayment()
        {
            PaymentStatus = "00";
        }
    }
}
