using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models
{
    [Table("RotationActvities", Schema = "public")]
    public class RotationActivity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)

        [ForeignKey("Workflow")]
        public long WorkflowId { get; set; }
        public Rotation Workflow { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }

        public String Info { get; set; }
        
        public long PreviousActivityId { get; set; }
        public long NextActivityId { get; set; }
        [ForeignKey("PreviousActivityId")]
        public RotationActivity PreviousActivity { get; set; }
        [ForeignKey("NextActivityId")]
        public RotationActivity NextActivity { get; set; }

        // PreviousAction
        // NextAction

        public String Name { get; set; }
        

        // AlteredTo
        // AlterDuration

        public bool IsFinished { get; set; }
        public bool HasDownloadAccess { get; set; }
        public bool HasPrintAccess { get; set; }
        public long TotalDownloaded { get; set; }
        public long TotalPrinted { get; set; }

        public String SubscriptionType { get; set; }
        
        public BusinessSubscription SubscriptionOf { get; set; }

    }
}
