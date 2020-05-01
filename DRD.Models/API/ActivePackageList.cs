using System.Collections.Generic;

namespace DRD.Models.API
{
    public class ActivePackageList
    {
        public ICollection<ActiveUsage> packages { set; get; }

        public long addSubscription(ActiveUsage package)
        {
            packages.Add(package);
            return package.Id;
        }

        public bool mergeBusinessSubscriptionList(ActiveUsageList BusinessSubscriptionList)
        {
            List<ActiveUsage> list = new List<ActiveUsage>();
            list.AddRange(packages);
            list.AddRange(BusinessSubscriptionList.usages);
            packages = list.ToArray();
            return packages != null;
        }

        public ActivePackageList()
        {
            packages = new List<ActiveUsage>();
        }
    }

    public class ActivePackage
    {
        public long Id { get; set; } // Id (Primary key)
        public string PackageName { get; set; } // MemberId
        public decimal? TotalPrice { get; set; } // Price
        public System.DateTime? StartedAt { get; set; } // ValidPackage
        public System.DateTime? ExpiredAt { get; set; } // ValidDrDrive
        public int? AdministratorsLimit { get; set; } // Id (Primary key)
        public int? UsersLimit { get; set; } // Id (Primary key)
        public int? RotationLimit { get; set; } // Id (Primary key)
        public int? WorkflowLimit { get; set; } // Id (Primary key)
        public long? StorageLimit { get; set; } // Id (Primary key)
        public bool? IsActive { get; set; } // IsDefault
    }
}