using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoNewsDetail
    {
        public long Id { get; set; } // Id (Primary key)
        public long NewsId { get; set; } // NewsId
        public string Image { get; set; } // Image (length: 50)
        public string Descr { get; set; } // Descr
    }
}
