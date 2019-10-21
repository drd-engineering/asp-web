using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberLogin
    {
        public long Id { get; set; } // Id (Primary key)
        public string Number { get; set; } // Number (length: 20)
        public int MemberTitleId { get; set; } // MemberTitleId
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 50)
        public int MemberType { get; set; } // MemberType
        public string KtpNo { get; set; } // KtpNo (length: 50)
        public string ImageProfile { get; set; } // ImageProfile (length: 50)
        public string ImageQrCode { get; set; } // ImageQrCode (length: 50)
        public System.DateTime? LastLogin { get; set; } // LastLogin
        public System.DateTime? LastLogout { get; set; } // LastLogout
        public long? ActivationKeyId { get; set; } // ActivationKeyId
        public long? CompanyId { get; set; } // CompanyId
        public string UserGroup { get; set; } // UserGroup (length: 20)
        public string ImageSignature { get; set; } // ImageSignature (length: 100)
        public string ImageInitials { get; set; } // ImageInitials (length: 100)
        public string ImageStamp { get; set; } // ImageStamp (length: 100)
        public string ImageKtp1 { get; set; } // ImageKtp1 (length: 100)
        public string ImageKtp2 { get; set; } // ImageKtp2 (length: 100)

        public string Password { get; set; }

        public DtoMemberTitle MemberTitle { get; set; } // FK_Member_MemberTitle
        public int SubscriptTypeId { get; set; } // SubscriptTypeId
        public string ShortName { get; set; }
        public DtoCompany Company { get; set; }
        public DtoSubscriptType SubscriptType { get; set; }
    }
}
