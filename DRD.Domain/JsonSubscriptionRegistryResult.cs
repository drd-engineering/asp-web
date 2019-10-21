using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonSubscriptionRegistryResult
    {
        public string SubscriptionId { get; set; }
        public string UserNumber { get; set; }
        public string Email { get; set; }

        public JsonSubscriptionRegistryResult(string subscriptionId, string userNumber, string email)
        {
            this.SubscriptionId = subscriptionId;
            this.UserNumber = userNumber;
            this.Email = email;
        }
        public JsonSubscriptionRegistryResult()
        {
            this.SubscriptionId = "";
            this.UserNumber = "";
            this.Email = "";
        }
    }
}
