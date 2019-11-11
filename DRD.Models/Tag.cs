using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Tags", Schema = "public")]
    public class Tag
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public Company Companies { get; set; } 
    }
}
