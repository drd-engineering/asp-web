using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoPaymentMethod
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 5)
        public string Name { get; set; } // Name (length: 50)
        public string Logo { get; set; } // Logo (length: 50)
        public string Descr { get; set; } // Descr (length: 1000)
        public int UsingType { get; set; } // UsingType
        public int ConfirmType { get; set; } // ConfirmType
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public System.Collections.Generic.IEnumerable<DtoCompanyBank> CompanyBanks { get; set; }


        public DtoPaymentMethod()
        {
            UsingType = 3;
            ConfirmType = 0;
        }
    }
}
