namespace DRD.Models.Custom
{
    public class UserSession
    {
        public long Id { get; set; } // Id (Primary key)
        public string FirstName { get; set; } // Name (length: 50)
        public string Email { get; set; }
        public string ProfileImageFileName { get; set; }
        public string EncryptedId { get; set; }
        public bool IsActive { get; set; }
        
        public UserSession(User userGet)
        {
            Id = userGet.Id;
            FirstName = userGet.Name.Split(' ')[0];
            ProfileImageFileName = userGet.ProfileImageFileName;
            EncryptedId = UtilitiesModel.Encrypt(userGet.Id.ToString());
            Email = userGet.Email;
            IsActive = userGet.IsActive;
        }
    }

}
