using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonPermissionWorkflow
    {
        public long Id { get; set; } // Id (Primary key)
        public long WorkflowId { get; set; }
        public string Name { get; set; } // Name (length: 100)
        public bool IsSelected { get; set; } // Selected 
        public bool IsAll { get; set; } // IsAll

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<JsonPermissionRotation> Rotations { get; set; }

    }
}
