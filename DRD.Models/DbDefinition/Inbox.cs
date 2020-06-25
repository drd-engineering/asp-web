using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DRD.Models;

namespace DRD.Models
{
    [Table("Inbox", Schema = "public")]
    public class Inbox
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } // Id (Primary key)
        public bool IsUnread { get; set; }
        public string Message { get; set; }
        public long RotationId { get; set; }

        [ForeignKey("Activity")]
        public long ActivityId { get; set; }
        public RotationNode Activity { get; set; }
        public string LastStatus { get; set; }
        public string prevUserName { get; set; }
        public string prevUserEmail { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }

        public System.DateTime CreatedAt { get; set; }
        public String DateNote { get; set; }

        public Inbox()
        {
            Id = UtilitiesModel.RandomLongGenerator(minimumValue: 1000000000);
        }
    }
}
