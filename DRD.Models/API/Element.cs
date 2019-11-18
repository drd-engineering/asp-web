using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    [NotMapped]
    public class Element
    {
        public long  UserId { get; set; }
        public string Name { get; set; }
        public string Foto { get; set; }
    }
}
