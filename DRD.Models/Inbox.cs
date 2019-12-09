using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DRD.Models
{
    [Table("Inbox", Schema = "public")]
    public class Inbox
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public bool IsUnread { get; set; }
        public String Message { get; set; }

        //[ForeignKey("Activity")]
        //public long ActivityId { get; set; }
        //public RotationActivity Activity { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }

        public System.DateTime CreatedAt;
        public String DateNote;

    }
}
