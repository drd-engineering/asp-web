using DRD.Models.API;

namespace DRD.Models.View
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
        public SmallCompanyData Company { get; set; }

        public MemberItem()
        {
            User = new ContactItem();
            Company = new SmallCompanyData();
        }
    }
}
