using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class Member
    {
        public long Id { get; set; } // Id (Primary key)
        
        public string Password { get; set; } // Password (length: 50)
        public long? CompanyId { get; set; } // CompanyId
        public bool IsActive { get; set; } // IsActive
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime JoinedAt { get; set; } // DateCreated

        
        public long LoginId { get; set; }

        public Member()
        {
            LoginId = 0;
            IsActive = true;
        }
    }
}
