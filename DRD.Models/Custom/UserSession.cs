using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.Custom
{
    public class UserSession
    {
        public long Id { get; set; } // Id (Primary key)
        public string EncryptedId { get; set; } // for folder destination location profile image
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 50)
        public long OfficialIdNo { get; set; }
        public string ImageProfile { get; set; }
        public string Password { get; set; } // Password (length: 20)
        public string ImageSignature { get; set; }
        public string ImageInitials { get; set; }
        public string ImageStamp { get; set; }
        public string ImageKtp1 { get; set; }
        public string ImageKtp2 { get; set; }
        public bool IsActive { get; set; }
        public long? ActivationKeyId { get; set; } // ActivationKeyId
        public long? CompanyId { get; set; } // CompanyId
        public int SubscriptTypeId { get; set; } // SubscriptTypeId
        public string ShortName { get; set; }
    }
}
