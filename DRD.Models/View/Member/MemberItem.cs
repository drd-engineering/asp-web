using DRD.Models.API.Contact;
using DRD.Models.API.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.Member
{
    public class MemberItem
    {
        public long Id { get; set; } // Id (Primary key)
        public long CompanyId { get; set; } // CompanyId
        public long UserId { get; set; } // UserId (length: 50)
        public System.DateTime JoinedAt { get; set; } // DateCreated
        public bool IsActive { get; set; } // IsActive
        public bool isCompanyAccept { get; set; }
        public bool isMemberAccept { get; set; }
        public bool IsAdministrator { get; set; } // IsActive
        
        public ContactItem User { get; set; }
        public CompanyItem Company{ get; set; }

        public MemberItem()
        {
            User = new ContactItem();
            Company = new CompanyItem();
        }
    }
}
