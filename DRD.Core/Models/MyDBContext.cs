using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DRD.Models;

namespace DRD.Core.Models
{
    public class MyDBContext : System.Based.Core.Entity.DrdContext
    {
        public MyDBContext()
        {

        }
        public DbSet<AnnotateType> annotateTypes { get; set; } 
        public DbSet<Company> companies { get; set; }
        
    }
}