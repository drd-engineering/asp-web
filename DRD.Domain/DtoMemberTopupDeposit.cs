using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberTopupDeposit
    {
        public long Id { get; set; } // Id (Primary key)
        public string TopupNo { get; set; } // TopupNo (length: 20)
        public System.DateTime TopupDate { get; set; } // TopupDate
        public long MemberId { get; set; } // MemberId
        public decimal Amount { get; set; } // Amount
        public string PaymentStatus { get; set; } // PaymentStatus (length: 2)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated


        public string PaymentStatusDescr { get; set; }
        public string MemberDescr { get; set; }
        public string KeyId { get; set; }

        public System.Collections.Generic.IEnumerable<DtoPaymentMethod> PaymentMethods { get; set; } // master pay method
        //public DtoCompanyBank CompanyBank { get; set; }

        // Reverse navigation
        public System.Collections.Generic.ICollection<DtoMemberTopupPayment> MemberTopupPayments { get; set; } // MemberTopupPayment.FK_MemberTopupPayment_MemberTopupDeposit

        // Foreign keys
        public DtoMember Member { get; set; } // FK_MemberTopupDeposit_Member

        public DtoMemberTopupDeposit()
        {
            MemberTopupPayments = new System.Collections.Generic.List<DtoMemberTopupPayment>();
        }
    }
}
