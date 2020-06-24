using DRD.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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
        public DbSet<DocumentElement> DocumentElements { get; set; }
        public DbSet<DocumentUser> DocumentUsers { get; set; }
        public DbSet<Inbox> Inboxes { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<RotationNode> RotationNodes { get; set; }
        public DbSet<RotationNodeDoc> RotationNodeDocs { get; set; }
        public DbSet<RotationNodeRemark> RotationNodeRemarks { get; set; }
        public DbSet<RotationNodeUpDoc> RotationNodeUpDocs { get; set; }
        public DbSet<RotationUser> RotationUsers { get; set; }
        public DbSet<Stamp> Stamps { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagItem> TagItems { get; set; }
        public DbSet<BusinessUsage> Usages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowNode> WorkflowNodes { get; set; }
        public DbSet<WorkflowNodeLink> WorkflowNodeLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<User> listOfUserCreated = new List<User>();
            listOfUserCreated.Add(new User
            {
                Id = -1,
                Name = "user 1",
                Phone = "081111111111",
                Email = "a@hotmail.com",
                OfficialIdNo = 1111111111111111,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -2,
                Name = "user 2",
                Phone = "081111111112",
                Email = "b@hotmail.com",
                OfficialIdNo = 1111111111111112,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -14,
                Name = "user 14",
                Phone = "081111111113",
                Email = "c@hotmail.com",
                OfficialIdNo = 1111111111111113,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -4,
                Name = "user 4",
                Phone = "081111111114",
                Email = "d@hotmail.com",
                OfficialIdNo = 1111111111111114,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -5,
                Name = "user 5",
                Phone = "081111111115",
                Email = "e@hotmail.com",
                OfficialIdNo = 1111111111111115,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -16,
                Name = "user 16",
                Phone = "081111111116",
                Email = "f@hotmail.com",
                OfficialIdNo = 1111111111111116,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -7,
                Name = "user 7",
                Phone = "081111111117",
                Email = "g@hotmail.com",
                OfficialIdNo = 1111111111111117,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -8,
                Name = "user 8",
                Phone = "081111111118",
                Email = "h@hotmail.com",
                OfficialIdNo = 1111111111111118,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -9,
                Name = "user 9",
                Phone = "081111111119",
                Email = "i@hotmail.com",
                OfficialIdNo = 1111111111111119,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -10,
                Name = "user 10",
                Phone = "081111111121",
                Email = "j@hotmail.com",
                OfficialIdNo = 1111111111111121,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
            });
            listOfUserCreated.Add(new User
            {
                Id = -11,
                Name = "user 11",
                Phone = "081111111122",
                Email = "k@hotmail.com",
                OfficialIdNo = 1111111111111122,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User
            {
                Id = -12,
                Name = "user 12",
                Phone = "081111111123",
                Email = "l@hotmail.com",
                OfficialIdNo = 1111111111111123,
                ImageProfile = "danilova.jpg",
                Password = "kapandeadline",
                ImageSignature = "danilovasgn.png",
                ImageInitials = "danilovainit.png",
                ImageStamp = "danilovastmp.png",
                ImageKtp1 = "danilovaktp1.png",
                ImageKtp2 = "danilovaktp2.png",
                IsActive = true,
                CreatedAt = DateTime.Now
            });
            modelBuilder.Entity<User>().HasData(listOfUserCreated[0], listOfUserCreated[1], listOfUserCreated[2], listOfUserCreated[3], listOfUserCreated[4], listOfUserCreated[5]);
            modelBuilder.Entity<User>().HasData(listOfUserCreated[6], listOfUserCreated[7], listOfUserCreated[8], listOfUserCreated[9], listOfUserCreated[10], listOfUserCreated[11]);

            List<Company> listOfCompanyCreated = new List<Company>();
            listOfCompanyCreated.Add(
                new Company
                {
                    Id = -1,
                    Code = "DG23JJKL3L",
                    Name = "PT perusahaan 1",
                    Phone = "0214556372",
                    Email = "sempoasupport@sempoa.com",
                    Descr = "a company dummy",
                    Address = "jalan hehe nomor 2, hehe, kota hehe, provinsi hehe",
                    PostalCode = "122122",
                    IsActive = true,
                    OwnerId = listOfUserCreated[0].Id,
                    CreatedAt = DateTime.Now
                });
            listOfCompanyCreated.Add(
                new Company
                {
                    Id = -2,
                    Code = "DG23JJ2PDO",
                    Name = "PT perusahaan 2",
                    Phone = "0218229103",
                    Email = "sempuasupport@sempua.com",
                    Descr = "a company dummy",
                    Address = "jalan haha nomor 2, haha, kota haha, provinsi haha",
                    PostalCode = "211211",
                    IsActive = true,
                    OwnerId = listOfUserCreated[2].Id,
                    CreatedAt = DateTime.Now
                });
            listOfCompanyCreated.Add(
                new Company
                {
                    Id = -4,
                    Code = "DG23JE4PDO",
                    Name = "PT perusahaan 4",
                    Phone = "0218229103",
                    Email = "siminsupport@sempua.com",
                    Descr = "a company dummy",
                    Address = "jalan huhu nomor 2, huhu, kota huhu, provinsi huhu",
                    PostalCode = "221211",
                    IsActive = true,
                    OwnerId = listOfUserCreated[4].Id,
                    CreatedAt = DateTime.Now
                });
            listOfCompanyCreated.Add(
                new Company
                {
                    Id = -5,
                    Code = "DG23JJKL3L",
                    Name = "PT perusahaan 5",
                    Phone = "0214556372",
                    Email = "sempoasupport@sempoa.com",
                    Descr = "a company dummy",
                    Address = "jalan hehe nomor 2, hehe, kota hehe, provinsi hehe",
                    PostalCode = "122122",
                    IsActive = true,
                    OwnerId = listOfUserCreated[0].Id,
                    CreatedAt = DateTime.Now
                });

            // Owner
            Member member6 = new Member
            {
                Id = -6,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[0].Id,
                IsAdministrator = false
            };
            Member member11 = new Member
            {
                Id = -11,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[2].Id,
                IsAdministrator = false
            };
            Member member13 = new Member
            {
                Id = -13,
                CompanyId = listOfCompanyCreated[2].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[4].Id,
                IsAdministrator = false
            };
            Member member26 = new Member
            {
                Id = -26,
                CompanyId = listOfCompanyCreated[3].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[0].Id,
                IsAdministrator = false
            };

            // Member biasa
            Member member1 = new Member
            {
                Id = -1,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[2].Id,
                IsAdministrator = true
            };
            Member member7 = new Member
            {
                Id = -7,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[4].Id,
                IsAdministrator = false
            };
            Member member8 = new Member
            {
                Id = -8,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[3].Id,
                IsAdministrator = false
            };
            Member member18 = new Member
            {
                Id = -18,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[1].Id,
                IsAdministrator = false
            };
            Member member19 = new Member
            {
                Id = -19,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[5].Id,
                IsAdministrator = false
            };
            Member member20 = new Member
            {
                Id = -20,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[6].Id,
                IsAdministrator = false
            };
            Member member21 = new Member
            {
                Id = -21,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[7].Id,
                IsAdministrator = false
            };
            Member member22 = new Member
            {
                Id = -22,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[8].Id,
                IsAdministrator = false
            };
            Member member23 = new Member
            {
                Id = -23,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[9].Id,
                IsAdministrator = false
            };
            Member member24 = new Member
            {
                Id = -24,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[10].Id,
                IsAdministrator = false
            };
            Member member25 = new Member
            {
                Id = -25,
                CompanyId = listOfCompanyCreated[0].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[11].Id,
                IsAdministrator = false
            };

            Member member2 = new Member
            {
                Id = -2,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[3].Id,
                IsAdministrator = false
            };
            Member member3 = new Member
            {
                Id = -3,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[0].Id,
                IsAdministrator = true
            };
            Member member4 = new Member
            {
                Id = -4,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[4].Id,
                IsAdministrator = false
            };
            Member member5 = new Member
            {
                Id = -5,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[5].Id,
                IsAdministrator = false
            };
            Member member15 = new Member
            {
                Id = -15,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[1].Id,
                IsAdministrator = false
            };
            Member member16 = new Member
            {
                Id = -16,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[6].Id,
                IsAdministrator = false
            };
            Member member17 = new Member
            {
                Id = -17,
                CompanyId = listOfCompanyCreated[1].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[7].Id,
                IsAdministrator = false
            };

            Member member9 = new Member
            {
                Id = -9,
                CompanyId = listOfCompanyCreated[2].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[5].Id,
                IsAdministrator = true
            };
            Member member10 = new Member
            {
                Id = -10,
                CompanyId = listOfCompanyCreated[2].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[0].Id,
                IsAdministrator = true
            };
            Member member12 = new Member
            {
                Id = -12,
                CompanyId = listOfCompanyCreated[2].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[3].Id,
                IsAdministrator = false
            };
            Member member14 = new Member
            {
                Id = -14,
                CompanyId = listOfCompanyCreated[2].Id,
                IsActive = true,
                isCompanyAccept = true,
                isMemberAccept = true,
                UserId = listOfUserCreated[4].Id,
                IsAdministrator = false
            };

            BusinessPackage package1 = new BusinessPackage { Id = -1, IsActive = true, IsExceedLimitAllowed = false, IsExpirationDateExtendedAutomatically = true, RotationStarted=-99,CreatedAt=DateTime.Now,IsPublic=true,Member=-99, Storage = 100000000, Administrator = 2, Duration = 60, Name = "Business" };
            BusinessPackage package2 = new BusinessPackage { Id = -2, IsActive = true, IsExceedLimitAllowed = false, IsExpirationDateExtendedAutomatically = true, RotationStarted = -99, CreatedAt = DateTime.Now, IsPublic = true, Member = -99, Storage = 100000000, Administrator = 2, Name = "Corporate" };
            BusinessPackage package3 = new BusinessPackage { Id = -3, IsActive = true, IsExceedLimitAllowed = false, IsExpirationDateExtendedAutomatically = true, RotationStarted = -99, CreatedAt = DateTime.Now, IsPublic = true, Member = -99, Storage = 100000000, Administrator = 2, Name = "Enterprise" };

            Price price1 = new Price { Id = -1, CreatedAt = DateTime.Now, Total = 2019192039, PackageId = package1.Id };
            Price price2 = new Price { Id = -2, CreatedAt = DateTime.Now, Total = 31241256, PackageId = package2.Id };
            Price price3 = new Price { Id = -3, CreatedAt = DateTime.Now, Total = 423566135, PackageId = package3.Id };

            BusinessUsage usage1 = new BusinessUsage { Id = -1, CompanyId = listOfCompanyCreated[0].Id, PackageId = package1.Id, StartedAt = DateTime.Now, ExpiredAt = DateTime.Now.AddDays(package1.Duration), PriceId = price1.Id };
            BusinessUsage usage2 = new BusinessUsage { Id = -2, CompanyId = listOfCompanyCreated[1].Id, PackageId = package2.Id, StartedAt = DateTime.Now, PriceId = price2.Id };
            BusinessUsage usage3 = new BusinessUsage { Id = -3, CompanyId = listOfCompanyCreated[2].Id, PackageId = package3.Id, StartedAt = DateTime.Now, PriceId = price3.Id };

            Contact contact1 = new Contact { ContactOwnerId = listOfUserCreated[0].Id, ContactItemId = listOfUserCreated[1].Id };
            Contact contact2 = new Contact { ContactOwnerId = listOfUserCreated[0].Id, ContactItemId = listOfUserCreated[2].Id };
            Contact contact3 = new Contact { ContactOwnerId = listOfUserCreated[1].Id, ContactItemId = listOfUserCreated[3].Id };
            Contact contact4 = new Contact { ContactOwnerId = listOfUserCreated[1].Id, ContactItemId = listOfUserCreated[4].Id };
            Contact contact5 = new Contact { ContactOwnerId = listOfUserCreated[1].Id, ContactItemId = listOfUserCreated[5].Id };
            Contact contact6 = new Contact { ContactOwnerId = listOfUserCreated[5].Id, ContactItemId = listOfUserCreated[6].Id };

            modelBuilder.Entity<Company>().HasData(listOfCompanyCreated[0], listOfCompanyCreated[1], listOfCompanyCreated[2], listOfCompanyCreated[3]);
            modelBuilder.Entity<Member>().HasData(member1, member2, member3, member4, member5, member6, member7, member8, member9, member10, member11, member12, member13, member14, member15, member16, member17, member18, member19, member20, member21, member22, member23, member24, member25, member26);

            modelBuilder.Entity<BusinessPackage>().HasData(package1, package2, package3);
            modelBuilder.Entity<Price>().HasData(price1, price2, price3);
            modelBuilder.Entity<BusinessUsage>().HasData(usage1, usage2, usage3);

            modelBuilder.Entity<Contact>().HasKey(c => new { c.ContactOwnerId, c.ContactItemId });
            modelBuilder.Entity<Contact>().HasData(contact1, contact2, contact3, contact4, contact5, contact6);
            modelBuilder.Entity<TagItem>().HasKey(e => new { e.TagId, e.RotationId });

            /*Workflow wf1 = new Workflow { Id = -1, CreatorId = listOfUserCreated[0].Id, DateCreated = DateTime.Now, IsActive = true, Name = "myone", Description = "cuy", IsTemplate = false, UserEmail = listOfUserCreated[0].Email };
            
            WorkflowNode wfn1 = new WorkflowNode { Id = -1, WorkflowId = wf1.Id, SymbolCode = 0, Caption = "Start", WorkflowNodeLinkTos = new List<WorkflowNodeLink>(), WorkflowNodeLinks = new List<WorkflowNodeLink>(), Value = "0", TextColor = "#ffffff", BackColor = "#008000" };
            WorkflowNode wfn2 = new WorkflowNode { Id = -2, WorkflowId = wf1.Id, SymbolCode = 1, Caption = "End", WorkflowNodeLinkTos = new List<WorkflowNodeLink>(), WorkflowNodeLinks = new List<WorkflowNodeLink>(), Value = "0", TextColor = "#ffffff", BackColor = "#ff0000" };
            WorkflowNode wfn3 = new WorkflowNode { Id = -3, WorkflowId = wf1.Id, SymbolCode = 5, Caption = "Activity", WorkflowNodeLinkTos = new List<WorkflowNodeLink>(), WorkflowNodeLinks = new List<WorkflowNodeLink>(), Value = "0", PosLeft = "0px", PosTop = "0px", TextColor = "#ffffff", BackColor = "#deb887" };

            WorkflowNodeLink wflnl1 = new WorkflowNodeLink { Id = -1, FirstNodeId = wfn3.Id, EndNodeId = wfn2.Id, WorkflowNodeId = wfn1.Id, Value = "0", WorkflowNodeToId = wfn3.Id, SymbolCode = 20 };
            WorkflowNodeLink wflnl2 = new WorkflowNodeLink { Id = -2, FirstNodeId = wfn3.Id, EndNodeId = wfn2.Id, WorkflowNodeId = wfn3.Id, Value = "0", WorkflowNodeToId = wfn2.Id, SymbolCode = 20 };

            modelBuilder.Entity<Workflow>().HasData(wf1);
            modelBuilder.Entity<WorkflowNode>().HasData(wfn1, wfn2, wfn3);
            modelBuilder.Entity<WorkflowNodeLink>().HasData(wflnl1, wflnl2);

            Rotation rt1 = new Rotation { Id = -1, CreatorId = listOfUserCreated[0].Id, Remark = "fe", Status = 1, WorkflowId = wf1.Id,
                Subject = "Goodddd", DateCreated = DateTime.Now, UserId = listOfUserCreated[0].Id};

            RotationNode rtn1 = new RotationNode
            {
                Id = -1,
                UserId = listOfUserCreated[2].Id,
                RotationId = rt1.Id,
                WorkflowNodeId = wfn3.Id,
                CreatedAt = DateTime.Now
            };

            RotationUser rtnusr1 = new RotationUser { Id = -1, isStartPerson = true, FlagPermission = 6, WorkflowNodeId = wfn3.Id, UserId = listOfUserCreated[1].Id, RotationId = rt1.Id };

            Inbox inbox1 = new Inbox
            {
                Id = -1,
                UserId = rtn1.UserId,
                ActivityId = rtn1.Id,
                Message = listOfUserCreated[2].Name + ", you has new Work on Rotatiion " + rt1.Subject,
                IsUnread = true,
                CreatedAt = DateTime.Now,
                DateNote = "New Created Inbox from " + rt1.Subject
            };

            rt1.DateUpdated = DateTime.Now;
            rt1.CompanyId = listOfCompanyCreated[2].Id;
            rt1.DateStarted = DateTime.Now;

            modelBuilder.Entity<Rotation>().HasData(rt1);
            modelBuilder.Entity<RotationNode>().HasData(rtn1);
            modelBuilder.Entity<RotationUser>().HasData(rtnusr1);
            modelBuilder.Entity<Inbox>().HasData(inbox1);
            */
        }
    }
}