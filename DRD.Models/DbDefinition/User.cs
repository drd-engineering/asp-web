using DRD.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Users", Schema = "public")]
    public class User : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Password { get; set; } // Password (length: 20)
        public bool PhoneConfirmed { get; set; } = false;
        public string Email { get; set; } // Email (length: 50)
        public bool EmailConfirmed { get; set; } = false;
        public string Username { get; set; } // Email (length: 50)
        public long OfficialIdNo { get; set; }
        public bool OfficialIdConfirmed { get; set; } = false;
        public string ProfileImageFileName { get; set; }
        public string SignatureImageFileName { get; set; }
        public string InitialImageFileName { get; set; }
        public string StampImageFileName { get; set; }
        public string KTPImageFileName { get; set; }
        public string KTPVerificationImageFileName { get; set; }
        public bool IsActive { get; set; } // IsActive

        public bool TwoFactorEnabled { get; set; } = false;
        public DateTime? LockOutEndDate{ get; set; } 
        public bool LockOutEnabled { get; set; } = false;
        public int AccessFailedCount { get; set; }


         public User()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: ConstantModel.MINIMUM_VALUE_ID, maximumValue: ConstantModel.MAXIMUM_VALUE_ID);
            Username = "" + Id;
        }
    }
}
