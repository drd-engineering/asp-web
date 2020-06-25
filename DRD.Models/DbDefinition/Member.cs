using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Members", Schema = "public")]
    public class Member
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public long CompanyId { get; set; } // CompanyId
        public long UserId { get; set; } // UserId (length: 50)
        public bool IsActive { get; set; } // IsActive
        public bool isCompanyAccept { get; set; }
        public bool isMemberAccept { get; set; }
        public bool IsAdministrator { get; set; } // IsActive
        public System.DateTime JoinedAt { get; set; } // DateMemberOfficiallyJoinedCompany
        public System.DateTime CreatedAt { get; set; } // FirstRequestMade
        public System.DateTime UpdatedAt { get; set; } // lastUpdates

        public Member()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: 1000000000);
            CreatedAt = System.DateTime.Now;
            UpdatedAt = System.DateTime.Now;
            IsAdministrator = false;
            IsActive = true;
        }
    }
}
