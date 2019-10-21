using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoDocumentMember
    {
        public long Id { get; set; } // Id (Primary key)
        public long DocumentId { get; set; } // DocumentId
        public long MemberId { get; set; } // MemberId
        public int FlagPermission { get; set; } // FlagPermission
        public int FlagAction { get; set; } // FlagAction

        public string MemberNumber { get; set; }
        public string MemberName { get; set; }

        //// Foreign keys
        //public virtual DtoDocument Document { get; set; } // FK_DocumentMember_Document
        //public virtual DtoMemberLite Member { get; set; } // FK_DocumentMember_Member

        public DtoDocumentMember()
        {
            FlagPermission = 0;
        }
    }
}
