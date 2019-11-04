using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class Plan
    {
        public long Id { get; set; } // Id (Primary key)
        public long MemberId { get; set; } // MemberId
        public int SubscriptTypeId { get; set; } // SubscriptTypeId
        public decimal Price { get; set; } // Price
        public string PriceUnitCode { get; set; } // PriceUnitCode (length: 10)
        public string PriceUnitDescr { get; set; } // PriceUnitDescr (length: 50)
        public int RotationCount { get; set; } // RotationCount
        public decimal RotationPrice { get; set; } // RotationPrice
        public int FlowActivityCount { get; set; } // FlowActivityCount
        public decimal FlowActivityPrice { get; set; } // FlowActivityPrice
        public long StorageSize { get; set; } // StorageSize
        public decimal StoragePrice { get; set; } // StoragePrice
        public long DrDriveSize { get; set; } // DrDriveSize
        public decimal DrDrivePrice { get; set; } // DrDrivePrice
        public int ExpiryDocDay { get; set; } // ExpiryDocDay
        public int PackageExpiryDay { get; set; } // PackageExpiryDay
        public int DrDriveExpiryDay { get; set; } // DrDriveExpiryDay
        public System.DateTime? ValidPackage { get; set; } // ValidPackage
        public System.DateTime? ValidDrDrive { get; set; } // ValidDrDrive
        public bool IsDefault { get; set; } // IsDefault
        public System.DateTime DateCreated { get; set; } // DateCreated

        public Plan()
        {
            Price = 0m;
            RotationCount = 0;
            RotationPrice = 0m;
            FlowActivityCount = 0;
            FlowActivityPrice = 0m;
            StoragePrice = 0m;
            DrDriveSize = 0;
            DrDrivePrice = 0m;
            ExpiryDocDay = 0;
            PackageExpiryDay = 0;
            DrDriveExpiryDay = 0;
            IsDefault = true;
        }
    }
}
