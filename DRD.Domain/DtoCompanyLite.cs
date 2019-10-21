using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoCompanyLite
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
        public string AliasName { get; set; } // AliasName (length: 20)
        public string Contact { get; set; } // Contact (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 100)
        public string SubAdminArea { get; set; } // SubAdminArea (length: 50)
        public bool IsActive { get; set; } // IsActive
        public int CompanyType { get; set; } // CompanyType
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated
       
    }
}
