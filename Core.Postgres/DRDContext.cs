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
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<DocumentHistory> DocumentHistories { get; set; }

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
            String passEncrypted = UtilitiesModel.Encrypt("drdaccess2021");
            String radikariPassEncrypted = UtilitiesModel.Encrypt("radikari2021");
            List <User> listOfUserCreated = new List<User>
            {
                new User
                {
                    Name = "Lalisa Tambolon",
                    Phone = "0897287837382",
                    Email = "lalisa@drdaccess.com",
                    OfficialIdNo = 2511100909080001,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Admin Radikari",
                    Phone = "08895737277",
                    Email = "superadmin@radikari.com",
                    OfficialIdNo = 2511100909080010,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Bam Viole",
                    Phone = "0897287837382",
                    Email = "bam@drdaccess.com",
                    OfficialIdNo = 2511100909080002,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Seraphina Alaydrus",
                    Phone = "0897287837382",
                    Email = "seraphina@drdaccess.com",
                    OfficialIdNo = 2511100909080003,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Chae Sinaga",
                    Phone = "085877213147",
                    Email = "chae@drdaccess.com",
                    OfficialIdNo = 2511100909080004,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Yeji Sitohang",
                    Phone = "081516322231",
                    Email = "yeji@drdaccess.com",
                    OfficialIdNo = 2511100909080005,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Yuna Siahahan",
                    Phone = "089756321121",
                    Email = "yuna@drdaccess.com",
                    OfficialIdNo = 2511100909080006,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Lia Simbolon",
                    Phone = "087731224234",
                    Email = "lia@drdaccess.com",
                    OfficialIdNo = 2511100909080007,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Momo Pakubumi",
                    Phone = "087865663123",
                    Email = "momo@drdaccess.com",
                    OfficialIdNo = 2511100909080008,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Palihan Panahan",
                    Phone = "0865526366232",
                    Email = "palihan@drdaccess.com",
                    OfficialIdNo = 2511100909080009,
                    Password = passEncrypted,
                    IsActive = true
                },
                new User
                {
                    Name = "Serina Selihan",
                    Phone = "08895737277",
                    Email = "serina@drdaccess.com",
                    OfficialIdNo = 2511100909080010,
                    Password = passEncrypted,
                    IsActive = true
                },
            };


            List<Company> listOfCompanyCreated = new List<Company>
            {
                new Company
                {
                    Code = "SOLTC202107001",
                    Name = "PT Artha Amita Sempurna",
                    Phone = "0214556372",
                    Email = "admin@drdaccess.com",
                    Description = "A trading and  contractor company",
                    Address = "jalan Musi 32, Gambir, Jakarta",
                    PostalCode = "10150",
                    IsActive = true,
                    IsVerified = true,
                    OwnerId = listOfUserCreated[0].Id,
                    CreatedAt = DateTime.Now
                },
                 new Company
                {
                    Code = "SOLTC202107001",
                    Name = "PT Rajawali Berdikari Indonesia",
                    Phone = "0214556372",
                    Email = "admin@radikari.com",
                    Description = "A trading and  contractor company",
                    Address = "The Great Saladin Square Blok C10, Jl. Margonda Raya No.39, Depok, Pancoran MAS, Depok, Kec. Pancoran Mas, Kota Depok, Jawa Barat 16431",
                    PostalCode = "10150",
                    IsActive = true,
                    IsVerified = true,
                    OwnerId = listOfUserCreated[1].Id,
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
                IsAdministrator = false
            };
            Member member4 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[3].Id,
                IsAdministrator = false
            };
            Member member5 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[4].Id,
                IsAdministrator = false
            };
            Member member6 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[5].Id,
                IsAdministrator = false
            };
            Member member7 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[6].Id,
                IsAdministrator = false
            };
            Member member8 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[7].Id,
                IsAdministrator = false
            };
            Member member9 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[8].Id,
                IsAdministrator = false
            };
            Member member10 = new Member
            {
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                IsCompanyAccept = true,
                IsMemberAccept = true,
                UserId = listOfUserCreated[9].Id,
                IsAdministrator = false
            };
            BusinessPackage package1 = new BusinessPackage { Id = -1, IsActive = true, IsExceedLimitAllowed = false, IsExpirationDateExtendedAutomatically = true, RotationStarted=-99,IsPublic=true,Member=-99, Storage = 100000000, Administrator = 2, Duration = 60, Name = "Business" };
            BusinessPackage package2 = new BusinessPackage { Id = -2, IsActive = true, IsExceedLimitAllowed = false, IsExpirationDateExtendedAutomatically = true, RotationStarted=-99,IsPublic=true,Member=-99, Storage = 10000000000, Administrator = 20, Duration = 90, Name = "Business" };
            
            Price price1 = new Price { Id = -1, CreatedAt = DateTime.Now, Total = 2019192039, PackageId = package1.Id };
            Price price2 = new Price { Id = -2, CreatedAt = DateTime.Now, Total = 2019192039, PackageId = package1.Id };
            
            BusinessUsage usage1 = new BusinessUsage { Id = -1, CompanyId = listOfCompanyCreated[0].Id, PackageId = package1.Id, CreatedAt = DateTime.Now, ExpiredAt = DateTime.Now.AddDays(package1.Duration), PriceId = price1.Id };
            BusinessUsage usage2 = new BusinessUsage { Id = -2, CompanyId = listOfCompanyCreated[1].Id, PackageId = package2.Id, CreatedAt = DateTime.Now, ExpiredAt = DateTime.Now.AddDays(package2.Duration), PriceId = price2.Id };


            modelBuilder.Entity<User>().HasData(listOfUserCreated[0], listOfUserCreated[1], listOfUserCreated[2], listOfUserCreated[3],
                listOfUserCreated[4], listOfUserCreated[5], listOfUserCreated[6], listOfUserCreated[7],
                listOfUserCreated[8], listOfUserCreated[9],listOfUserCreated[10]);
            modelBuilder.Entity<Company>().HasData(listOfCompanyCreated[0], listOfCompanyCreated[1]);
            modelBuilder.Entity<Member>().HasData(member1, member2, member3, member4, member5, member6, member7, member8, member9, member10);

            modelBuilder.Entity<BusinessPackage>().HasData(package1);
            modelBuilder.Entity<Price>().HasData(price1);
            modelBuilder.Entity<BusinessUsage>().HasData(usage1);

            modelBuilder.Entity<Contact>().HasKey(c => new { c.ContactOwnerId, c.ContactItemId });
            modelBuilder.Entity<TagItem>().HasKey(e => new { e.TagId, e.RotationId });
        }
    }
}