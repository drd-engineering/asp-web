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
        public DbSet<Member> Members { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAdmin> UserAdmins { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowNode> WorkflowNodes { get; set; }
        public DbSet<WorkflowNodeLink> WorkflowNodeLinks { get; set; }
        // public DbSet<RotationMember> RotationMembers { get; set; }
        public DbSet<RotationNode> RotationNodes { get; set; }
        public DbSet<RotationNodeDoc> RotationNodeDocs { get; set; }
        public DbSet<RotationNodeRemark> RotationNodeRemarks { get; set; }
        public DbSet<RotationNodeUpDoc> RotationNodeUpDocs { get; set; }
        public DbSet<DocumentSign> DocumentSigns { get; set; }
        // public DbSet<FaspayData> FaspayDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List < User > listOfUserCreated = new List<User>();
            listOfUserCreated.Add(new User { Id = 11111111, Name = "aminudin bin saleh", Phone = "085140451404", Email = "r@hotmail.com",
                OfficialIdNo = 3511101010101010, ImageProfile = "hehehehe.png", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = 11111121, Name = "amanudin bin saleh", Phone = "085858585858", Email = "a@hotmail.com",
                OfficialIdNo = 3511202020202020, ImageProfile = "hahahaha.png", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = 11111211, Name = "amirudin bin saleh", Phone = "085151515151", Email = "n@hotmail.com",
                OfficialIdNo = 3511303030303030, ImageProfile = "huhuhuhu.png", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = 11112111, Name = "amiradin bin saleh", Phone = "085888811111", Email = "i@hotmail.com",
                OfficialIdNo = 3511404040404040, ImageProfile = "hohohoho.png", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            modelBuilder.Entity<User>().HasData( listOfUserCreated[0], listOfUserCreated[1], listOfUserCreated[2], listOfUserCreated[3] );

            List<Company> listOfCompanyCreated = new List<Company>();
            listOfCompanyCreated.Add(
                new Company{Id = 1111112, Code = "DG23JJKL3L", Name = "PT SEMPOA", Phone = "0214556372", Email = "sempoasupport@sempoa.com", 
                    Descr = "a company dummy", Address = "jalan hehe nomor 2, hehe, kota hehe, provinsi hehe", PostalCode = "122122",  IsActive = true, 
                    OwnerId = listOfUserCreated[0].Id, CreatedAt = DateTime.Now});
            listOfCompanyCreated.Add(
                new Company{Id = 1111122, Code = "DG23JJ2PDO", Name = "PT SEMPUA", Phone = "0218229103", Email = "sempuasupport@sempua.com", 
                    Descr = "a company dummy", Address = "jalan haha nomor 2, haha, kota haha, provinsi haha", PostalCode = "211211",  IsActive = true, 
                    OwnerId = listOfUserCreated[1].Id, CreatedAt = DateTime.Now});
            Member member1 = new Member();
            member1.Id = 11212;
            member1.CompanyId = listOfCompanyCreated[0].Id;
            member1.IsActive = true;
            member1.UserId = listOfUserCreated[2].Id;

            Member member2 = new Member();
            member2.Id = 12212;
            member2.CompanyId = listOfCompanyCreated[1].Id;
            member2.IsActive = true;
            member2.UserId = listOfUserCreated[3].Id;

            Member member3 = new Member();
            member3.Id = 13212;
            member3.CompanyId = listOfCompanyCreated[1].Id;
            member3.IsActive = true;
            member3.UserId = listOfUserCreated[1].Id;

            modelBuilder.Entity<Member>().HasData(member1, member2, member3);
            modelBuilder.Entity<Company>().HasData(listOfCompanyCreated[0], listOfCompanyCreated[1]);

        }
    }
}
