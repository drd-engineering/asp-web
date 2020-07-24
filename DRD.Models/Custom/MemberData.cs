using System.Collections.Generic;

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
    public class MemberDataComparer : IEqualityComparer<MemberData>
    {
        public bool Equals(MemberData g1, MemberData g2)
        {
            return g1.Id == g2.Id;
        }

        public int GetHashCode(MemberData obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}