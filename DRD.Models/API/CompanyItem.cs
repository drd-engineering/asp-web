using DRD.Models.View;
using System.Collections.Generic;

namespace DRD.Models.API
{
    public class CompanyItem
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
        public string OwnerName { get; set; } // Name (length: 50)
        public long OwnerId { get; set; } // OwnerId (ForeignKey to user)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 100)
        public string Descr { get; set; } // Descr
        public string Address { get; set; } // Address (length: 1000)
        public string PointLocation { get; set; } // PointLocation (length: 1000)
        public string PostalCode { get; set; } // PostalCode (length: 5)
        public long SubscriptionId { get; set; } // PostalCode (length: 5)
        public string SubscriptionName { get; set; } // PostalCode (length: 5)
        public string Image1 { get; set; } // Image1 (length: 100)
        public string Image2 { get; set; } // Image2 (length: 100)
        public string ImageCard { get; set; } // ImageCard (length: 100)
        public bool IsActive { get; set; } // IsActive
        public bool IsVerified { get; set; } // IsVerified
        public long TotalMember { get; set; }
        public bool IsOwnedByUser { get; set; }
        public bool IsManagedByUser { get; set; }

        public List<MemberItem> Administrators { get; set; }
        public List<Member> Members { get; set; }

    }
}
