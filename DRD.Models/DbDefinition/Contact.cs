using DRD.Models.View;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Contacts", Schema = "public")]
    public class Contact : BaseEntity
    {
        [Key, Column(Order = 0)]
        public long ContactOwnerId { get; set; }
        [Key, Column(Order = 1)]
        public long ContactItemId { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }

        [ForeignKey("ContactOwnerId")]
        public virtual User ContactOwner { get; set; }
        [ForeignKey("ContactItemId")]
        public virtual User ContactItem { get; set; }
        public Contact()
        {

        }
        public Contact(long userId, long itemId)
        {
            ContactOwnerId = userId;
            ContactItemId = itemId;
            IsActive = true;
            IsBlocked = false;
        }

        public static implicit operator Contact(ContactItem v)
        {
            throw new NotImplementedException();
        }
    }
}
