using System;

namespace DRD.Models.View
{
    // Union of Member and User Request and Response?
    public class UserProfile
    {
        public long Id { get; set; } // Id (Primary key)
        public string EncryptedId { get; set; } // for folder destination location profile image
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public bool PhoneConfirmed { get; set; } = false;
        public string Email { get; set; } // Email (length: 50)
        public bool EmailConfirmed { get; set; } = false;

        public string ProfileImageFileName { get; set; }
        public string SignatureImageFileName { get; set; }
        public string InitialImageFileName { get; set; }
        public string StampImageFileName { get; set; }
        public string KTPImageFileName { get; set; }
        public string KTPVerificationImageFileName { get; set; }
        public string Username { get; set; } // Email (length: 50)
        public long OfficialIdNo { get; set; }
        public bool OfficialIdConfirmed { get; set; } = false;

        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; } // IsActive

        public bool TwoFactorEnabled { get; set; } = false;
        public UserProfile() { }
        public UserProfile(User user)
        {
            Id = user.Id;
            EncryptedId = UtilitiesModel.Encrypt(user.Id.ToString());
            Name = user.Name;
            Phone = user.Phone;
            PhoneConfirmed = user.PhoneConfirmed;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;
            ProfileImageFileName = user.ProfileImageFileName;
            SignatureImageFileName = user.SignatureImageFileName;
            InitialImageFileName = user.InitialImageFileName;
            StampImageFileName = user.StampImageFileName;
            KTPImageFileName = user.KTPImageFileName;
            KTPVerificationImageFileName = user.KTPVerificationImageFileName;
            Username = user.Username;
            OfficialIdNo = user.OfficialIdNo;
            OfficialIdConfirmed = user.OfficialIdConfirmed;
            CreatedAt = user.CreatedAt;
            IsActive = user.IsActive;
            TwoFactorEnabled = user.TwoFactorEnabled;
        }
    }
}
