using DRD.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Users", Schema = "public")]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public bool PhoneConfirmed { get; set; } = false;
        public string Email { get; set; } // Email (length: 50)
        public bool EmailConfirmed { get; set; } = false;
        public string UserName { get; set; } // Email (length: 50)
        public long OfficialIdNo { get; set; }
        public string ImageProfile { get; set; }
        public string Password { get; set; } // Password (length: 20)
        public string ImageSignature { get; set; }
        public string ImageInitials { get; set; }
        public string ImageStamp { get; set; }
        public string ImageKtp1 { get; set; }
        public string ImageKtp2 { get; set; }
        public bool IsActive { get; set; } // IsActive
        public DateTime CreatedAt { get; set; } // DateCreated

        public bool TwoFactorEnabled { get; set; } = false;
        public DateTime LockOutEndDate{ get; set; } 
        public bool LockOutEnabled { get; set; } = false;
        public int AccessFailedCount { get; set; }


         public User()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: ConstantModel.MINIMUM_VALUE_ID);
            UserName = "" + Id;
        }
    }
}
