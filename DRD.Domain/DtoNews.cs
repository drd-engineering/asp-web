using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoNews
    {
        public long Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 100)
        public string Descr { get; set; } // Descr
        public int NewsType { get; set; } // NewsType
        public bool IsActive { get; set; } // IsActive
        public long CreatorId { get; set; } // CreatorId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<DtoNewsDetail> NewsDetails { get; set; } // NewsDetail.FK_NewsDetail_News

        public DtoNewsMaster Master { get; set; }

        public DtoNews()
        {
            NewsType = 0;
            NewsDetails = new System.Collections.Generic.List<DtoNewsDetail>();
            Master = new DtoNewsMaster();
        }
    }
}
