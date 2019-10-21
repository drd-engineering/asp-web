using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoNewsLite
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 50)
        public int NewsType { get; set; } // NewsType
        public bool IsActive { get; set; } // IsActive
        public long CreatorId { get; set; } // CreatorId
        public string Descr { get; set; } // Descr
        public string Image { get; set; } // Image
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
    }
}
