using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoProjectLite
    {
        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 100)
        public string Descr { get; set; } // Descr
        public long CompanyId { get; set; } // CompanyId
        public bool IsActive { get; set; } // IsActive
        public long? CreatorId { get; set; } // CreatorId
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        //// Reverse navigation
        //public virtual System.Collections.Generic.ICollection<DtoWorkflow> Workflows { get; set; } // Workflow.FK_Workflow_Project

        //// Foreign keys
        //public virtual DtoCompany Company { get; set; } // FK_Project_Company

        public string CompanyName { get; set; }

        public DtoProjectLite()
        {
            // Workflows = new System.Collections.Generic.List<DtoWorkflow>();
        }
    }
}
