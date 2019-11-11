using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Members", Schema = "public")]
    public class Member
    {
        public long Id { get; set; } // Id (Primary key)
        
        public long? CompanyId { get; set; } // CompanyId
        public long? UserId { get; set; } // UserId (length: 50)
        public System.DateTime JoinedAt { get; set; } // DateCreated
        public bool IsActive { get; set; } // IsActive
        public bool IsAdministrator { get; set; } // IsActive

        public Member()
        {
            IsAdministrator = false;
            IsActive = true;
        }
    }
}
