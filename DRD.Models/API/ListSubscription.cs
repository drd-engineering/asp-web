using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    public class ListSubscription
    {
        public List<SubscriptionData> items { get; set; }
        public ListSubscription()
        {
            items = new List<SubscriptionData>();
        }
    }
    public class SubscriptionData
    {
        public long id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public long companyId { get; set; }
        public string companyName { get; set; }
    }
}
