using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DRD.Domain;
using System.Based.Core.Entity;

namespace Vidly.Models
{
    public class MyDBContext : DrdContext
    {
        public MyDBContext()
        {

        }
        public DbSet<DtoAnnotateType> Customers { get; set; } // My domain models
        public DbSet<DtoBank> Movies { get; set; }// My domain models
    }
}