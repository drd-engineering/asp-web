using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class FaspayData
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string MerchantId { get; set; }

        public string BCAKlikPayCode { get; set; }
        public string BCAClearKey { get; set; }

        public bool IsProduction { get; set; }
    }
}
