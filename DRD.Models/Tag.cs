using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Tags", Schema = "public")]
    public class Tag
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; } 
        public Company Companies { get; set; } 
    }
}
