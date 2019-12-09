using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Service
{
    public class SubscriptionDictionary
    {
        public Dictionary<int, Subscription> getBusinessSubscription = new Dictionary<int, Subscription>()
        {
            {1, new Subscription(){Id = 1, Name = "Standard", Price = 10500000, DurationInDays = 30, AdministratorQuota = 3, StorageQuotaInByte = 100^9} },
            {2, new Subscription(){Id = 2, Name = "Business", Price = 15000000, DurationInDays = 30, AdministratorQuota = 5, StorageQuotaInByte = 250^9} },
            {3, new Subscription(){Id = 3, Name = "Enterprise", Price = 27500000, DurationInDays = 30, AdministratorQuota = 10, StorageQuotaInByte = 1000^9} }
        };
    }

    public class Subscription
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public long DurationInDays { get; set; }
        public int AdministratorQuota { get; set; }
        public long StorageQuotaInByte { get; set; }
        DateTime createdAt { get; set; }

    }
}
