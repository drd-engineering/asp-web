using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("TagItems", Schema = "public")]
    public class TagItem : BaseEntity
    {
        [Key]
        [Column(Order = 1)]
        public int TagId { get; set; }
        [Key]
        [Column(Order = 2)]
        public long RotationId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
        [ForeignKey("RotationId")]
        public virtual Rotation Rotation { get; set; }
    }
}
