using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoNewsVideo
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 20)
        public string Title { get; set; } // Title (length: 100)
        public string Descr { get; set; } // Descr (length: 100)
        public string ChannelId { get; set; } // ChannelId (length: 20)
        public string ChannelTitle { get; set; } // ChannelTitle (length: 50)
        public long? CategoryId { get; set; } // CategoryId
        public System.DateTime DatePublished { get; set; } // DatePublished
        public bool IsActive { get; set; } // IsActive
        public long CreatorId { get; set; } // CreatorId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public DtoNewsVideo()
        {
            DateCreated = System.DateTime.Now;
        }
    }
}
