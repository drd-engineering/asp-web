using DRD.Models.View.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    public class BusinessSubscriptionList
    {
        public ICollection<BusinessSubscriptionItem> subscriptions { set; get; }

        public long addSubscription(BusinessSubscriptionItem BusinessSubscriptionItem)
        {
            subscriptions.Add(BusinessSubscriptionItem);
            return BusinessSubscriptionItem.Id;
        }
        public bool mergeBusinessSubscriptionList(BusinessSubscriptionList BusinessSubscriptionList)
        {
            List<BusinessSubscriptionItem> list = new List<BusinessSubscriptionItem>();
            list.AddRange(subscriptions);
            list.AddRange(BusinessSubscriptionList.subscriptions);
            subscriptions = list.ToArray();
            return subscriptions != null;
        }
        public BusinessSubscriptionList()
            {
            subscriptions = new List<BusinessSubscriptionItem>();
            }
    }
    public class BusinessSubscriptionItem
    {
        public long Id { get; set; } // Id (Primary key)
        public long? CompanyId { get; set; } // Id (Primary key)
        public string CompanyName { get; set; }
        public string SubscriptionName { get; set; } // MemberId
        public decimal? Price { get; set; } // Price
        public System.DateTime? StartedAt { get; set; } // ValidPackage
        public System.DateTime? ExpiredAt { get; set; } // ValidDrDrive
        public bool? IsActive { get; set; } // IsDefault
        public long? StorageUsedinByte { get; set; } // Id (Primary key)
        public int? totalAdministrators { get; set; } // Id (Primary key)
        public long? StorageRemainingInBytes { get; set; }

    }
}
