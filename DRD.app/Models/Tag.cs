using DRD.app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.app.Models
{
    public class Tag
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public Company Companies { get; set; } 
    }
}
