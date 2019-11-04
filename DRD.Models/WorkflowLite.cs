using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class WorkflowLite
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 100)
        public string Descr { get; set; } // Descr
        public long ProjectId { get; set; } // ProjectId
        public bool IsActive { get; set; } // IsActive
        public bool IsTemplate { get; set; } // IsTemplate
        public int WfType { get; set; } // WfType
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public string ProjectName { get; set; }

        public WorkflowLite()
        {
        }
    }
}
