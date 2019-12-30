﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("PlanBusinesses", Schema = "public")]
    public class PlanBusiness
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long CompanyId { get; set; } // Id (Primary key)
        public string SubscriptionName { get; set; } // MemberId
        public decimal Price { get; set; } // Price
        public System.DateTime? StartedAt { get; set; } // ValidPackage
        public System.DateTime? ExpiredAt { get; set; } // ValidDrDrive
        public bool IsActive { get; set; } // IsDefault
        public long StorageSize { get; set; } // StorageSize
        public long StorageUsedinByte { get; set; } // Id (Primary key)
        public int totalAdministrators { get; set; } // Id (Primary key)

        public PlanBusiness()
        {
            Price = 0m;
            IsActive = true;
        }
    }
}
