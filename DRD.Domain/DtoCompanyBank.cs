using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoCompanyBank
    {
        public int Id { get; set; } // Id (Primary key)
        public int BankId { get; set; } // BankId
        public string Branch { get; set; } // Branch (length: 100)
        public string AccountNo { get; set; } // AccountNo (length: 50)
        public string AccountName { get; set; } // AccountName (length: 50)
        public int PaymentMethodId { get; set; } // PaymentMethodId
        public bool IsActive { get; set; } // IsActive
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string KeyId { get; set; }

        //public string Code { get; set; } // Code (length: 5)
        //public int UsingType { get; set; } // UsingType
        //public int ConfirmType { get; set; } // ConfirmType


        // Foreign keys
        public DtoBank Bank { get; set; } // FK_CompanyBank_Bank
        public DtoPaymentMethod PaymentMethod { get; set; } // FK_CompanyBank_PaymentMethod

        public DtoCompanyBank()
        {
            IsActive = true;
            Bank = new DtoBank();
            PaymentMethod = new DtoPaymentMethod();
        }
    }
}
