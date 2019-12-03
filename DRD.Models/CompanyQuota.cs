using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace DRD.Models
{
    [Table("CompanyQuotas", Schema = "public")]
    public class CompanyQuota
    {
        public long Id { get; set; } // Id (Primary key)
        public long CompanyId { get; set; } // Id (Primary key)

        public int AdministratorQuota { get; set; }
        public long StorageQuota { get; set; }
        public int StorageUsage { get; set; }
        public DateTime startedAt { get; set; }
        public DateTime expiredAt { get; set; }
        public long price { get; set; }

        public long BusinessSubscriptionId { get; set; }
        public bool isActive { get; set; }


        public CompanyQuota()
        {
            
        }
    }
}
