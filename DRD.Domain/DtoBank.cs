using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoBank
    {
        public int Id { get; set; } // Id (Primary key) 
        public string Code { get; set; } // Code (length: 20)
        public string Name { get; set; } // Name (length: 50)
        public string Logo { get; set; } // Logo (length: 50)
        public int BankType { get; set; } // BankType
        public string UserId { get; set; } // UserId (length: 20)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        
        public DtoBank()
        {
            BankType = 1;
        }
    }
}
