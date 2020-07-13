using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DRD.Models;

namespace DRD.Models
{
    [Table("Inbox", Schema = "public")]
    public class Inbox : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public bool IsUnread { get; set; }
        public string Message { get; set; }
        public string Note { get; set; }
        public long RotationId { get; set; }

        [ForeignKey("Activity")]
        public long ActivityId { get; set; }
        public RotationNode Activity { get; set; }
        public string LastStatus { get; set; }
        public string PreviousUserName { get; set; }
        public string PreviousUserEmail { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }


        public Inbox()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID,Constant.MAXIMUM_VALUE_ID);
        }
    }
}
