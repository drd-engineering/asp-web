using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonPermissionCompany
    {
        public long Id { get; set; } // Id (Primary key)
        public long CompanyId { get; set; }
        public string Name { get; set; } // Name (length: 50)
        public bool IsSelected { get; set; } // Selected 
        public bool IsAll { get; set; } // IsAll 

        public virtual System.Collections.Generic.ICollection<JsonPermissionProject> Projects { get; set; }
    }
}
