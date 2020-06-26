using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DRD.Models
{
    [Table("Companies", Schema = "public")]
    public class Company : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
        public long OwnerId { get; set; } // OwnerId (ForeignKey to user)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 100)
        public string Description { get; set; } // Descr
        public string Address { get; set; } // Address (length: 1000)
        public string MapCoordinate { get; set; } // PointLocation (length: 1000)
        public string PostalCode { get; set; } // PostalCode (length: 5)
        public string ImageLogoUrl { get; set; } // Image1 (length: 100)
        public bool IsActive { get; set; } // IsActive
        public bool IsVerified { get; set; } // IsVerified

        public virtual ICollection<Member> Members { get; set; } // Member.FK_Member_Company

        public Company()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: 1000000000);
            IsActive = true;
            IsVerified = false;
            CreatedAt = DateTime.Now;
            Members = new List<Member>();
        }
    }
}
