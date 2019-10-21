using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoSubscriptType
    {
        public int Id { get; set; } // Id (Primary key)
        public string TypeCode { get; set; } // TypeCode (length: 10)
        public string ClassName { get; set; } // ClassName (length: 100)
        public string Descr { get; set; } // Descr (length: 1000)
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
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string StrStorageSize { get; set; } // StorageSize
        public string StrDrDriveSize { get; set; } // DrDriveSize

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<DtoMemberPlan> MemberPlans { get; set; } // MemberPlan.FK_MemberPlan_SubscriptType
        public virtual System.Collections.Generic.ICollection<DtoMemberPlanRequest> MemberPlanRequests { get; set; } // MemberPlanRequest.FK_MemberPlanRequest_SubscriptType

        public DtoSubscriptType()
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
            MemberPlans = new System.Collections.Generic.List<DtoMemberPlan>();
            MemberPlanRequests = new System.Collections.Generic.List<DtoMemberPlanRequest>();
        }
    }
}
