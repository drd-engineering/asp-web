using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonPermissionProject
    {
        public long Id { get; set; } // Id (Primary key)
        public long ProjectId { get; set; }
        public string Name { get; set; } // Name (length: 100)
        public bool IsSelected { get; set; } // Selected 
        public bool IsAll { get; set; } // IsAll

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<JsonPermissionWorkflow> Workflows { get; set; }
    }
}
