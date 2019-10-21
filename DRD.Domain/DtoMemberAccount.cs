using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberAccount
    {
        public long Id { get; set; } // Id (Primary key)
        public long MemberId { get; set; } // MemberId
        public string Title { get; set; } // Title (length: 20)
        public string AccountNo { get; set; } // AccountNo (length: 50)
        public string AccountName { get; set; } // AccountName (length: 50)
        public int BankId { get; set; } // BankId
        public string Branch { get; set; } // Branch (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string KeyId { get; set; }

        // Foreign keys
        public DtoBank Bank { get; set; } // FK_MemberAccount_Bank
        public DtoMember Member { get; set; } // FK_MemberAccount_Member
    }
}
