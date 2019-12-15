using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.View;

namespace DRD.Models
{
    [Table("Contacts", Schema = "public")]
    public class Contact
    {
        public long Id { get; set; }
        public long ContactOwnerId { get; set; }
        public long ContactItemId { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime RemovedAt { get; set; }

        [ForeignKey("ContactOwnerId")]
        public virtual User ContactOwner { get; set; }
        [ForeignKey("ContactItemId")]
        public virtual User ContactItem { get; set; }

        public static implicit operator Contact(ContactItem v)
        {
            throw new NotImplementedException();
        }
    }
}
