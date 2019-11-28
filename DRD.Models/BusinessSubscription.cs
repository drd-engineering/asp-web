using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("BusinessSubscriptions", Schema = "public")]
    public class BusinessSubscription
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
