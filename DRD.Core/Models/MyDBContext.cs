using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DRD.Models;


namespace Vidly.Models
{
    public class MyDBContext : DrdContext
    {
        public MyDBContext()
        {

        }
        public DbSet<AnnotateType> Customers { get; set; } // My domain models
        public DbSet<Company> Movies { get; set; }// My domain models
    }
}