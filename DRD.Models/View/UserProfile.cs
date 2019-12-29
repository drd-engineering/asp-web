using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    // Union of Member and User Request and Response?
    public class UserProfile
    {
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 50)

        public string ImageProfile { get; set; }
        public string ImageSignature { get; set; }
        public string ImageInitials { get; set; }
        public string ImageStamp { get; set; }
        public string ImageKtp1 { get; set; }
        public string ImageKtp2 { get; set; }
        public long OfficialIdNo { get; set; }
        
        public string Password { get; set; } // Password (length: 20)

        public DateTime CreatedAt { get; set; }
        
        public bool IsActive { get; set; } // IsActive

        //public long? CompanyId { get; set; }
    }
}
