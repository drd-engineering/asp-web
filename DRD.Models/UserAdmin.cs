using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class UserAdmin
    {
        public long Id { get; set; } // Id (Primary key)
        public string Email { get; set; } // Email (length: 50)
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public int AdminType { get; set; } // AdminType
        public string AppZoneAccess { get; set; } // AppZoneAccess (length: 50)
        public string Password { get; set; } // Password (length: 20)
        public bool IsActive { get; set; } // IsActive
        public int PanelType { get; set; } // PanelType
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        

        public UserAdmin() {
            AdminType = 0;
            IsActive = true;
        }
    }
}
