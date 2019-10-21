using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberPlan
    {
        public long Id { get; set; } // Id (Primary key)
        public long MemberId { get; set; } // MemberId
        public int SubscriptTypeId { get; set; } // SubscriptTypeId
        public decimal Price { get; set; } // Price
        public string PriceUnitCode { get; set; } // PriceUnitCode (length: 10)
        public string PriceUnitDescr { get; set; } // PriceUnitDescr (length: 50)
        public int RotationCount { get; set; } // RotationCount
        public decimal RotationPrice { get; set; } // RotationPrice
        public int RotationCountAdd { get; set; } // RotationCountAdd
        public int RotationCountUsed { get; set; } // RotationCountUsed
        public int FlowActivityCount { get; set; } // FlowActivityCount
        public decimal FlowActivityPrice { get; set; } // FlowActivityPrice
        public int FlowActivityCountAdd { get; set; } // FlowActivityCountAdd
        public int FlowActivityCountUsed { get; set; } // FlowActivityCountUsed
        public long StorageSize { get; set; } // StorageSize
        public decimal StoragePrice { get; set; } // StoragePrice
        public long StorageSizeAdd { get; set; } // StorageSizeAdd
        public long StorageSizeUsed { get; set; } // StorageSizeUsed
        public long DrDriveSize { get; set; } // DrDriveSize
        public decimal DrDrivePrice { get; set; } // DrDrivePrice
        public long DrDriveSizeAdd { get; set; } // DrDriveSizeAdd
        public long DrDriveSizeUsed { get; set; } // DrDriveSizeUsed
        public int ExpiryDocDay { get; set; } // ExpiryDocDay
        public int PackageExpiryDay { get; set; } // PackageExpiryDay
        public int DrDriveExpiryDay { get; set; } // DrDriveExpiryDay
        public System.DateTime? ValidPackage { get; set; } // ValidPackage
        public System.DateTime? ValidDrDrive { get; set; } // ValidDrDrive
        public bool IsDefault { get; set; } // IsDefault
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string StrStorageSize { get; set; } // StorageSize
        public string StrStorageSizeAdd { get; set; } // StorageSizeAdd
        public string StrStorageSizeUsed { get; set; } // StorageSizeUsed
        public string StrStorageSizeBal { get; set; } // StorageSizeUsed

        public string StrDrDriveSize { get; set; } // DrDriveSize
        public string StrDrDriveSizeAdd { get; set; } // DrDriveSizeAdd
        public string StrDrDriveSizeUsed { get; set; } // DrDriveSizeUsed
        public string StrDrDriveSizeBal { get; set; } // DrDriveSizeUsed

        // Foreign keys
        public virtual DtoMember Member { get; set; } // FK_MemberPlan_Member
        public virtual DtoSubscriptType SubscriptType { get; set; } // FK_MemberPlan_SubscriptType

        public DtoMemberPlan()
        {
            Price = 0m;
            RotationCount = 0;
            RotationPrice = 0m;
            RotationCountAdd = 0;
            RotationCountUsed = 0;
            FlowActivityCount = 0;
            FlowActivityPrice = 0m;
            FlowActivityCountAdd = 0;
            FlowActivityCountUsed = 0;
            StoragePrice = 0m;
            StorageSizeAdd = 0;
            StorageSizeUsed = 0;
            DrDriveSize = 0;
            DrDrivePrice = 0m;
            DrDriveSizeAdd = 0;
            DrDriveSizeUsed = 0;
            ExpiryDocDay = 0;
            PackageExpiryDay = 0;
            DrDriveExpiryDay = 0;
            IsDefault = true;
        }
    }
}
