using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoSubscriptExtraType
    {
        public int Id { get; set; } // Id (Primary key)
        public decimal Price { get; set; } // Price
        public int RotationCount { get; set; } // RotationCount
        public int FlowActivityCount { get; set; } // FlowActivityCount
        public long StorageSize { get; set; } // StorageSize
        public long DrDriveSize { get; set; } // DrDriveSize
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string StrStorageSize { get; set; } // StorageSize
        public string StrDrDriveSize { get; set; } // DrDriveSize

        public DtoSubscriptExtraType()
        {
            Price = 0m;
            RotationCount = 0;
            FlowActivityCount = 0;
            DrDriveSize = 0;
        }
    }
}
