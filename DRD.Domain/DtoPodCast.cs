using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoPodCast
    {
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 100)
        public string Descr { get; set; } // Descr (length: 100)
        public int Duration { get; set; } // Duration 
        public string Image { get; set; } // Image (length: 100)
        public string AudioFileName { get; set; } // AudioFileName (length: 100)
        public string FileNameOri { get; set; } // FileNameOri (length: 100)
        public bool IsActive { get; set; } // IsActive
        public long CreatorId { get; set; } // CreatorId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public DtoPodCast()
        {
            IsActive = true;
        }
    }
}
