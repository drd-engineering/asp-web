using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Members", Schema = "public")]
    public class Member : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public long CompanyId { get; set; } // CompanyId
        public long UserId { get; set; } // UserId (length: 50)
        public bool IsActive { get; set; } // IsActive
        public bool IsCompanyAccept { get; set; }
        public bool IsMemberAccept { get; set; }
        public bool IsAdministrator { get; set; } // IsActive
        public System.DateTime? JoinedAt { get; set; } // DateMemberOfficiallyJoinedCompany

        public Member()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: ConstantModel.MINIMUM_VALUE_ID, maximumValue: ConstantModel.MAXIMUM_VALUE_ID);
            CreatedAt = System.DateTime.Now;
            UpdatedAt = System.DateTime.Now;
            IsAdministrator = false;
            IsActive = true;
        }
    }
}
