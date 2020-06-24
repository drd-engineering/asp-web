using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("BusinessUsages", Schema = "public")]
    public class BusinessUsage
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)

        public bool IsActive { get; set; } // IsDefault
        public long PriceId { get; set; }
        public DateTime ExpiredAt { get; set; } = DateTime.MaxValue;
        public DateTime StartedAt { get; set; }
        public int Administrator { get; set; } = 0;
        public int RotationStarted { get; set; } = 0;
        public int Member { get; set; } = 0;
        public long CompanyId { get; set; } // Id (Primary key)
        public long PackageId { get; set; }
        public long Storage { get; set; } = 0;

        public BusinessUsage()
        {
            IsActive = true;
        }
        public BusinessUsage(BusinessUsage usage, DateTime startedAt, DateTime expiredAt)
        {
            IsActive = true;
            PriceId = usage.PriceId;
            StartedAt = startedAt;
            ExpiredAt = expiredAt;
            Administrator = usage.Administrator;
            RotationStarted = 0;
            Member = usage.Member;
            CompanyId = usage.CompanyId;
            PackageId = usage.PackageId;
            Storage = 0;
        }
    }
}