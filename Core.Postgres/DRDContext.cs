using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DRD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Core.Postgres
{
    public class DRDContext : DbContext
    {
        public DRDContext(DbContextOptions<DRDContext> options) : base(options) { }

        public DbSet<ElementType> ElementTypes { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentElement> DocumentElements { get; set; }
       // public DbSet<DocumentSign> DocumentSigns { get; set; }
       // public DbSet<FaspayData> FaspayDatas { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        // public DbSet<RotationMember> RotationMembers { get; set; }
        // public DbSet<RotationNode> RotationNodes { get; set; }
        // public DbSet<RotationNodeDoc> RotationNodeDocs { get; set; }
        // public DbSet<RotationNodeRemark> RotationNodeRemarks { get; set; }
        // public DbSet<RotationNodeUpDoc> RotationNodeUpDocs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAdmin> UserAdmins { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        // public DbSet<WorkflowNode> WorkflowNodes { get; set; }
        // public DbSet<WorkflowNodeLink> WorkflowNodeLinks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List < User > listOfUserCreated = new List<User>();
            listOfUserCreated.Add(new User { Id = 11111111, Name = "aminudin bin saleh", Phone = "085140451404", Email = "aminudin.saleh@hotmail.com",
                OfficialIdNo = 3511101010101010, ImageProfile = "hehehehe.png", Password = "hXKFKEfd!!jafIenJ@42FDNKDjdaknfj",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = 11111121, Name = "amanudin bin saleh", Phone = "085858585858", Email = "amanudin.saleh@hotmail.com",
                OfficialIdNo = 3511202020202020, ImageProfile = "hahahaha.png", Password = "hX83KEfd!!jaJFKnJ@42FDNKDjKDLnfj",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = 11111211, Name = "amirudin bin saleh", Phone = "085151515151", Email = "amirudin.saleh@hotmail.com",
                OfficialIdNo = 3511303030303030, ImageProfile = "huhuhuhu.png", Password = "hXk2O1fd!!jafIJpw)92FDNKDjdaknfj",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = 11112111, Name = "amiradin bin saleh", Phone = "085888811111", Email = "amiradin.saleh@hotmail.com",
                OfficialIdNo = 3511404040404040, ImageProfile = "hohohoho.png", Password = "hLoe90fd!!jaJFKnJ@42FDnf@3kDLnfj",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            modelBuilder.Entity<User>().HasData( listOfUserCreated[0], listOfUserCreated[1], listOfUserCreated[2], listOfUserCreated[3] );

            List<Company> listOfCompanyCreated = new List<Company>();
            listOfCompanyCreated.Add(
                new Company{Id = 1111112, Code = "DG23JJKL3L", Name = "PT SEMPOA", Contact = Phone Email Descr Address PointLocation PostalCode Image1 Image2 ImageCard IsActive UserId
                CreatedAt  ICollection<Member> Members Tags});

            modelBuilder.Entity<Company>().HasData( );
        }
    }
}
