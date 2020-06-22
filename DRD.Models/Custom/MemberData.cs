namespace DRD.Models.Custom
{
    public class MemberData
    {
        public long Id { get; set; } // Id (Primary key)
        public string EncryptedId { get; set; } // generate use Generator encryption
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 50)
        public string ImageProfile { get; set; } // ImageProfile (length: 50)
        //public int MemberType { get; set; }
        //public string UserGroup { get; set; }
        public string CompanyName { get; set; }

        public MemberData() { }
    }
}