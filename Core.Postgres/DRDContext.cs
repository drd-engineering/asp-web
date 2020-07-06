using DRD.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Postgres
{
    public class DRDContext : DbContext
    {
        public DRDContext(DbContextOptions<DRDContext> options) : base(options)
        {
        }

        public DbSet<BusinessPackage> BusinessPackages { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentAnnotation> DocumentElements { get; set; }
        public DbSet<DocumentUser> DocumentUsers { get; set; }
        public DbSet<Inbox> Inboxes { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<RotationNode> RotationNodes { get; set; }
        public DbSet<RotationNodeDoc> RotationNodeDocs { get; set; }
        public DbSet<RotationUser> RotationUsers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagItem> TagItems { get; set; }
        public DbSet<BusinessUsage> Usages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowNode> WorkflowNodes { get; set; }
        public DbSet<WorkflowNodeLink> WorkflowNodeLinks { get; set; }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.Now; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<User> listOfUserCreated = new List<User>
            {
                new User
                {
                    Name = "Lalisa Tambolon",
                    Phone = "0897287837382",
                    Email = "lalisa@solobhakti.com",
                    OfficialIdNo = 2511100909080001,
                    ProfileImageFileName = "user.jpg",
                    Password = "solobhakti2020",
                    SignatureImageFileName = "",
                    InitialImageFileName = "",
                    StampImageFileName = "",
                    KTPImageFileName = "",
                    KTPVerificationImageFileName = "",
                    IsActive = true
                },
                new User
                {
                    Name = "Bam Viole",
                    Phone = "0897287837382",
                    Email = "bam@solobhakti.com",
                    OfficialIdNo = 2511100909080001,
                    ProfileImageFileName = "user.jpg",
                    Password = "solobhakti2020",
                    SignatureImageFileName = "",
                    InitialImageFileName = "",
                    StampImageFileName = "",
                    KTPImageFileName = "",
                    KTPVerificationImageFileName = "",
                    IsActive = true
                },
                new User
                {
                    Name = "Seraphina Alaydrus",
                    Phone = "0897287837382",
                    Email = "seraphina@solobhakti.com",
                    OfficialIdNo = 2511100909080001,
                    ProfileImageFileName = "user.jpg",
                    Password = "solobhakti2020",
                    SignatureImageFileName = "",
                    InitialImageFileName = "",
                    StampImageFileName = "",
                    KTPImageFileName = "",
                    KTPVerificationImageFileName = "",
                    IsActive = true
                },
                new User
                {
                    Name = "Arlo Iskandar",
                    Phone = "0897287837382",
                    Email = "arlo@solobhakti.com",
                    OfficialIdNo = 2511100909080001,
                    ProfileImageFileName = "user.jpg",
                    Password = "solobhakti2020",
                    SignatureImageFileName = "",
                    InitialImageFileName = "",
                    StampImageFileName = "",
                    KTPImageFileName = "",
                    KTPVerificationImageFileName = "",
                    IsActive = true
                },

            };

            modelBuilder.Entity<User>().HasData(listOfUserCreated[0], listOfUserCreated[1], listOfUserCreated[2], listOfUserCreated[3]);

            List<Company> listOfCompanyCreated = new List<Company>
            {
                new Company
                {
                    Code = "SOLTC202007001",
                    Name = "PT Solobhakti Trading & Contractor",
                    Phone = "0214556372",
                    Email = "sempoasupport@sempoa.com",
                    Description = "A trading and  contractor company",
                    Address = "jalan Musi 32, Gambir, Jakarta",
                    PostalCode = "10150",
                    IsActive = true,
                    IsVerified = true,
                    OwnerId = listOfUserCreated[0].Id,
                    CreatedAt = DateTime.Now
                },
             
            };

            Member member1 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[0].Id,
                IsAdministrator = false
            };

            Member member2 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[1].Id,
                IsAdministrator = true
            };
            Member member3 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[2].Id,
                IsAdministrator = true
            };
            Member member4 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[3].Id,
                IsAdministrator = true
            };

            BusinessPackage package1 = new BusinessPackage { Id = -1, IsActive = true, IsExceedLimitAllowed = false, IsExpirationDateExtendedAutomatically = true, RotationStarted=-99,IsPublic=true,Member=-99, Storage = 100000000, Administrator = 2, Duration = 60, Name = "Business" };
            Price price1 = new Price { Id = -1, CreatedAt = DateTime.Now, Total = 2019192039, PackageId = package1.Id };
            BusinessUsage usage1 = new BusinessUsage { Id = -1, CompanyId = listOfCompanyCreated[0].Id, PackageId = package1.Id, CreatedAt = DateTime.Now, ExpiredAt = DateTime.Now.AddDays(package1.Duration), PriceId = price1.Id };


            modelBuilder.Entity<Company>().HasData(listOfCompanyCreated[0]);
            modelBuilder.Entity<Member>().HasData(member1, member2, member3, member4);

            modelBuilder.Entity<BusinessPackage>().HasData(package1);
            modelBuilder.Entity<Price>().HasData(price1);
            modelBuilder.Entity<BusinessUsage>().HasData(usage1);

            modelBuilder.Entity<Contact>().HasKey(c => new { c.ContactOwnerId, c.ContactItemId });
            modelBuilder.Entity<TagItem>().HasKey(e => new { e.TagId, e.RotationId });

        }
    }
}