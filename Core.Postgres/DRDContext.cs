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
        public DbSet<CompanyQuota> CompanyQuotas { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentElement> DocumentElements { get; set; }
        public DbSet<BusinessSubscription> BusinessSubscriptions { get; set; }

        public DbSet<Member> Members { get; set; }
        public DbSet<PlanBusiness> PlanBusinesses { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<User> Users { get; set; }
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

        //public DbSet<RotationActivity> RotationActvities { get; set; }
        public DbSet<Inbox> Inbox { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List < User > listOfUserCreated = new List<User>();
            listOfUserCreated.Add(new User { Id = 11111111, Name = "aminudin bin saleh", Phone = "085140451404", Email = "r@hotmail.com",
                OfficialIdNo = 3511101010101010, ImageProfile = "hehehehe.png", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = 11111121, Name = "amanudin bin saleh", Phone = "085858585858", Email = "a@hotmail.com",
                OfficialIdNo = 3511202020202020, ImageProfile = "no_picture.png", Password = "kapandeadline",
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
            
            Member member1 = new Member { Id = 11212, CompanyId = listOfCompanyCreated[1].Id, IsActive = true, UserId = listOfUserCreated[2].Id };

            Member member2 = new Member { Id = 12212, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, UserId = listOfUserCreated[3].Id};
            
            Member member3 = new Member { Id = 13212, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, UserId = listOfUserCreated[1].Id };

            Member member4 = new Member { Id = 14212, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, UserId = listOfUserCreated[0].Id, IsAdministrator = true};

            Member member5 = new Member { Id = 14312, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, UserId = listOfUserCreated[2].Id };

            Contact contact1 = new Contact { ContactOwnerId = listOfUserCreated[0].Id, ContactItemId = listOfUserCreated[1].Id};
            Contact contact2 = new Contact { ContactOwnerId = listOfUserCreated[0].Id, ContactItemId = listOfUserCreated[2].Id};

            modelBuilder.Entity<Member>().HasData(member1, member2, member3, member4, member5);
            modelBuilder.Entity<Contact>().HasData(contact1, contact2);
            modelBuilder.Entity<Contact>().HasKey(c => new { c.ContactOwnerId, c.ContactItemId });
            listOfCompanyCreated[0].OwnerId = member2.Id;
            modelBuilder.Entity<Company>().HasData(listOfCompanyCreated[0], listOfCompanyCreated[1]);
        }
    }
}
