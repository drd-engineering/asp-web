using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberInvited
    {
        public long Id { get; set; } // Id (Primary key)
        public long MemberId { get; set; } // MemberId
        public long InvitedId { get; set; } // InvitedId
        public string Status { get; set; } // Status (length: 2)
        public System.DateTime DateExpiry { get; set; } // DateExpiry
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string StatusDescr { get; set; }
        public virtual DtoMemberLite Member { get; set; }
        public virtual DtoMemberLite Invited { get; set; }

        //// Foreign keys
        //public virtual DtoMember Member_InvitedId { get; set; } // FK_MemberInvited_Invited
        //public virtual DtoMember Member_MemberId { get; set; } // FK_MemberInvited_Member

        public DtoMemberInvited()
        {
            Status = "00";
        }
    }
}
