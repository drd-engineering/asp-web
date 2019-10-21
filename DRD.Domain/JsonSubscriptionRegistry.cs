using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonSubscriptionRegistry
    {
        public int MemberTitleId { get; set; }
        public string MemberName { get; set; }
        public string MemberPhone { get; set; }
        public string MemberEmail{ get; set; }
        public int SubscriptTypeId { get; set; }

        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPointLocation { get; set; }
        public decimal Latitude { get; set; } 
        public decimal Longitude { get; set; }
    }
}
