using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonMemberRegister
    {
        public String Number { get; set; } // Number (length: 50)
        public String Name { get; set; } // Name (length: 50)
        public String Email { get; set; } // Email (length: 50)
        public String Phone { get; set; } // Phone (length: 20)
        public String Password { get; set; } // Phone (length: 20)
    }
}
