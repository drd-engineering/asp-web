using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.Company
{
    public class CompanyDetail
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
        public long TotalMember { get; set; }
        public int limitAdmin { get; set; }
        public long OwnerId { get; set; }
        public bool isVerified { get; set; }
        public string Phone { get; set; } // Phone (length: 20)
        public string Address { get; set; } // Address (length: 1000)
        public string PointLocation { get; set; } // PointLocation (length: 1000)
        public List<DRD.Models.Member> Members { get; set; }
        public List<DRD.Models.Member> Administrators { get; set; }
        public CompanyDetail()
        {
            Members = new List<DRD.Models.Member>();
            Administrators = new List<DRD.Models.Member>();
        }
    }
}
