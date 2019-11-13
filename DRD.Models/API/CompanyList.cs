using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API.Register
{
    public class CompanyList
    {
        public ICollection<Company> companies{set; get;}
    }
    public class Company{
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
    }
}
