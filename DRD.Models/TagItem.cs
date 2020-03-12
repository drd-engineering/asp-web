using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("TagItems", Schema = "public")]
    public class TagItem
    {
        public int TagId { get; set; } 
        public string RotationId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
        [ForeignKey("Rotation")]
        public virtual Rotation Rotation { get; set; }
    }
}
