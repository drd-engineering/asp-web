using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Inbox", Schema = "public")]
    class Inbox
    {
        public long Id;
        public Boolean IsUnread;
        public String Message;

        // public [?] ActivityId;
        // public [RotationActivity] Userid;

        public System.DateTime CreatedAt;
        public String DateNote;

    }
}
