using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoNewsVideoLite
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 20)
        public string Title { get; set; } // Title (length: 100)
        public bool IsActive { get; set; } // IsActive
        public long CreatorId { get; set; } // CreatorId
        public System.DateTime DatePublished { get; set; } // DatePublished
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        
    }
}
