using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonUserMaster
    {
        public int Id { get; set; } // ID (Primary key)
        public string UserId { get; set; } // UserID (length: 20)
        public string UserName { get; set; } // UserName (length: 50)
        public string Password { get; set; } // Password (length: 500)
        public int UserTypeId { get; set; } // UserTypeID
        public int? GroupMasterId { get; set; } // GroupMasterID
        public string CompanyCode { get; set; } // CompanyCode (length: 20)
    }
}
