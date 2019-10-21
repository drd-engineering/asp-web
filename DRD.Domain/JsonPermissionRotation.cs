using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonPermissionRotation
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationId { get; set; }
        public string Subject { get; set; } // Subject (length: 100)
        public bool IsSelected { get; set; } // Selected 
    }
}
