using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Prices", Schema = "public")]
    public class Price : BaseEntity
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)

        public long AdministratorExceeded { get; set; } = -99;
        public long UserExceeded { get; set; } = -99;
        public long RotationExceeded { get; set; } = -99;
        public long RotationStartedExceeded { get; set; } = -99;
        public long WorkflowExceeded { get; set; } = -99;
        public long StorageExceeded { get; set; } = -99;
        public long Total { get; set; }
        public string Currency { get; set; } = "IDR";
        public long PackageId { get; set; }
        public bool IsActive { get; set; } // IsDefault

        public Price()
        {
            IsActive = true;
        }
    }
}