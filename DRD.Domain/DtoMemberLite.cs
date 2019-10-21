using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberLite
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Number { get; set; } // Number (length: 20)
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 50)
        public string ImageProfile { get; set; } // ImageProfile (length: 50)
        public string ImageQrCode { get; set; } // ImageQrCode (length: 50)

        public string Profession { get; set; }
        public int MemberType { get; set; }
        public string UserGroup { get; set; }
        public string CompanyName { get; set; }

        public DtoMemberLite()
        {

        }
    }
}
