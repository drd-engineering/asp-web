using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonWorkflowProject
    {
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 100)
        public string Descr { get; set; } // Descr
    }
}
