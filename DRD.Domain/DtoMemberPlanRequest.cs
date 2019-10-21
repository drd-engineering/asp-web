using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberPlanRequest
    {
        public long Id { get; set; } // Id (Primary key)
        public long MemberId { get; set; } // MemberId
        public int SubscriptTypeId { get; set; } // SubscriptTypeId
        public int RotationCount { get; set; } // RotationCount
        public int FlowActivityCount { get; set; } // FlowActivityCount
        public long StorageSize { get; set; } // StorageSize
        public long DrDriveSize { get; set; } // DrDriveSize
        public int ExpiryDocDay { get; set; } // ExpiryDocDay
        public int PackageExpiryDay { get; set; } // PackageExpiryDay
        public string Status { get; set; } // Status (length: 2)
        public System.DateTime DateCreated { get; set; } // DateCreated

        // Foreign keys
        public virtual DtoMember Member { get; set; } // FK_MemberPlanRequest_Member
        public virtual DtoSubscriptType SubscriptType { get; set; } // FK_MemberPlanRequest_SubscriptType

        public DtoMemberPlanRequest()
        {
            RotationCount = 0;
            FlowActivityCount = 0;
            DrDriveSize = 0;
            ExpiryDocDay = 0;
            PackageExpiryDay = 0;
        }
    }
}
