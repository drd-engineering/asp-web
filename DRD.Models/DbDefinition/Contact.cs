using DRD.Models.View;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Contacts", Schema = "public")]
    public class Contact
    {
        [Key, Column(Order = 0)]
        public long ContactOwnerId { get; set; }
        [Key, Column(Order = 1)]
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
