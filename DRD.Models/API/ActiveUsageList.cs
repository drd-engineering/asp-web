using System.Collections.Generic;

namespace DRD.Models.API
{
    public class ActiveUsage
    {
        public int? AdministratorsLimit { get; set; }
        public long? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public System.DateTime? ExpiredAt { get; set; }
        public long Id { get; set; }
        public bool? IsActive { get; set; } // IsDefault
        public string PackageName { get; set; }
        public int? RotationStartedLimit { get; set; }
        public System.DateTime? StartedAt { get; set; }
        public long? StorageLimit { get; set; }
        public int? TotalAdministrators { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? TotalRotationStarted { get; set; }
        public long? TotalStorage { get; set; }

        public int? TotalUsers { get; set; }
        public int? TotalWorkflow { get; set; }
        public int? UsersLimit { get; set; }
    }

    public class ActiveUsageList
    {
        public ActiveUsageList()
        {
            usages = new List<ActiveUsage>();
        }

        public ICollection<ActiveUsage> usages { set; get; }

        public long addSubscription(ActiveUsage usages)
        {
            this.usages.Add(usages);
            return usages.Id;
        }

        public bool mergeBusinessSubscriptionList(ActiveUsageList usages)
        {
            List<ActiveUsage> list = new List<ActiveUsage>();
            list.AddRange(this.usages);
            list.AddRange(usages.usages);
            this.usages = list.ToArray();
            return this.usages != null;
        }
    }
}