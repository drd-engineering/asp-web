using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("AuditTrails", Schema = "public")]
    public class AuditTrail : BaseEntity
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public long UserId { get; set; }
        public string Activity { get; set; }
        public int Type { get; set; }
    }
}