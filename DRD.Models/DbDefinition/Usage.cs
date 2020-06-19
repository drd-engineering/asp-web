using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Usages", Schema = "public")]
    public class Usage
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)

        public bool IsActive { get; set; } // IsDefault
        public long PriceId { get; set; }
        public DateTime ExpiredAt { get; set; } = DateTime.MaxValue;
        public DateTime StartedAt { get; set; }
        public int Administrator { get; set; } = 0;
        public int Rotation { get; set; } = 0;
        public int RotationStarted { get; set; } = 0;
        public int User { get; set; } = 0;
        public int Workflow { get; set; } = 0;
        public long CompanyId { get; set; } // Id (Primary key)
        public long PackageId { get; set; }
        public long Storage { get; set; } = 0;

        public Usage()
        {
            IsActive = true;
        }
    }
}