namespace DRD.Models.Custom
{
    public class UserSession
    {
        public long Id { get; set; } // Id (Primary key)
        public string EncryptedId { get; set; } // for folder destination location profile image
        public string FirstName { get; set; } // Name (length: 50)
        public string FullName { get; set; }
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
        
        public UserSession(User userGet)
        {
            Id = userGet.Id;
            EncryptedId = UtilitiesModel.Encrypt(userGet.Id.ToString());
            FirstName = userGet.Name.Split(' ')[0];
            OfficialIdNo = userGet.OfficialIdNo;
            Phone = userGet.Phone;
            Email = userGet.Email;
            ImageProfile = userGet.ProfileImageFileName;
            ImageSignature = userGet.SignatureImageFileName;
            ImageInitials = userGet.InitialImageFileName;
            ImageStamp = userGet.StampImageFileName;
            ImageKtp1 = userGet.KTPImageFileName;
            ImageKtp2 = userGet.KTPVerificationImageFileName;
            FullName = userGet.Name;
        }
    }

}
