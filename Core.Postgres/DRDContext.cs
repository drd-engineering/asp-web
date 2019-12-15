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

        public DbSet<Member> Members { get; set; }
        public DbSet<PlanBusiness> PlanBusinesses { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<UserAdmin> UserAdmins { get; set; }
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
            List<User> listOfUserCreated = new List<User>();
            listOfUserCreated.Add(new User { Id = -1, Name = "aminudin bin saleh", Phone = "085140451404", Email = "a@hotmail.com",
                OfficialIdNo = 3511101010101010, ImageProfile = "danilova.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = -2, Name = "amanudin bin saleh", Phone = "085858585858", Email = "r@hotmail.com",
                OfficialIdNo = 3511202020202020, ImageProfile = "danilova.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = -3, Name = "amirudin bin saleh", Phone = "085151515151", Email = "n@hotmail.com",
                OfficialIdNo = 3511303030303030, ImageProfile = "ann.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = -4, Name = "amiradin bin saleh", Phone = "085888811111", Email = "i@hotmail.com",
                OfficialIdNo = 3511404040404040, ImageProfile = "danilova.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = -5, Name = "amir bin saleh", Phone = "085151515151", Email = "q@hotmail.com",
                OfficialIdNo = 3511303030303030, ImageProfile = "ann.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = -6, Name = "adin bin saleh", Phone = "085888811111", Email = "w@hotmail.com",
                OfficialIdNo = 3511404040404040, ImageProfile = "danilova.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = -7, Name = "mirud bin saleh", Phone = "085151515151", Email = "e@hotmail.com",
                OfficialIdNo = 3511303030303030, ImageProfile = "ann.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            listOfUserCreated.Add(new User { Id = -8, Name = "din bin saleh", Phone = "085888811111", Email = "p@hotmail.com",
                OfficialIdNo = 3511404040404040, ImageProfile = "danilova.jpg", Password = "kapandeadline",
                ImageSignature = null, ImageInitials = null, ImageStamp = null, ImageKtp1 = null, ImageKtp2 = null,
                IsActive = true, CreatedAt = DateTime.Now
            });
            modelBuilder.Entity<User>().HasData(listOfUserCreated[0], listOfUserCreated[1], listOfUserCreated[2], listOfUserCreated[3], listOfUserCreated[4], listOfUserCreated[5], listOfUserCreated[6]);

            List<Company> listOfCompanyCreated = new List<Company>();
            listOfCompanyCreated.Add(
                new Company { Id = -1, Code = "DG23JJKL3L", Name = "PT AMARTHA INDAH SENTOSA", Phone = "0214556372", Email = "sempoasupport@sempoa.com",
                    Descr = "a company dummy", Address = "jalan hehe nomor 2, hehe, kota hehe, provinsi hehe", PostalCode = "122122", IsActive = true,
                    OwnerId = listOfUserCreated[0].Id, CreatedAt = DateTime.Now });
            listOfCompanyCreated.Add(
                new Company { Id = -2, Code = "DG23JJ2PDO", Name = "PT SEMPUA", Phone = "0218229103", Email = "sempuasupport@sempua.com",
                    Descr = "a company dummy", Address = "jalan haha nomor 2, haha, kota haha, provinsi haha", PostalCode = "211211", IsActive = true,
                    OwnerId = listOfUserCreated[2].Id, CreatedAt = DateTime.Now });
            listOfCompanyCreated.Add(
                new Company { Id = -3, Code = "DG23JE4PDO", Name = "PT simin TIGI RIDI", Phone = "0218229103", Email = "siminsupport@sempua.com",
                    Descr = "a company dummy", Address = "jalan huhu nomor 2, huhu, kota huhu, provinsi huhu", PostalCode = "221211", IsActive = true,
                    OwnerId = listOfUserCreated[4].Id, CreatedAt = DateTime.Now });

            Member member1 = new Member { Id = -1, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, isCompanyAccept = true,
                isMemberAccept = true, UserId = listOfUserCreated[2].Id, IsAdministrator = true  };
            Member member2 = new Member { Id = -2, CompanyId = listOfCompanyCreated[1].Id, IsActive = true, isCompanyAccept = true,
                isMemberAccept = true, UserId = listOfUserCreated[3].Id, IsAdministrator = false  };
            Member member3 = new Member { Id = -3, CompanyId = listOfCompanyCreated[1].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[0].Id, IsAdministrator = true };
            Member member4 = new Member { Id = -4, CompanyId = listOfCompanyCreated[1].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[4].Id,IsAdministrator = false };
            Member member5 = new Member { Id = -5, CompanyId = listOfCompanyCreated[1].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[5].Id,IsAdministrator = false };
            Member member6 = new Member { Id = -6, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[0].Id,IsAdministrator = false };
            Member member7 = new Member { Id = -7, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[4].Id,IsAdministrator = false };
            Member member8 = new Member { Id = -8, CompanyId = listOfCompanyCreated[0].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[3].Id,IsAdministrator = false };
            Member member9 = new Member { Id = -9, CompanyId = listOfCompanyCreated[2].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[5].Id, IsAdministrator = true  };
            Member member10 = new Member { Id = -10, CompanyId = listOfCompanyCreated[2].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[0].Id, IsAdministrator = true  };
            Member member11 = new Member { Id = -11, CompanyId = listOfCompanyCreated[2].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[2].Id,IsAdministrator = false };
            Member member12 = new Member { Id = -12, CompanyId = listOfCompanyCreated[2].Id, IsActive = true, isCompanyAccept = true, 
                isMemberAccept = true, UserId = listOfUserCreated[3].Id,IsAdministrator = false
            };

            PlanBusiness planBusiness1 = new PlanBusiness { Id = -1, IsActive=true, CompanyId= listOfCompanyCreated[1].Id, Price=210000, ExpiredAt=DateTime.Now.AddDays(30), StartedAt=DateTime.Now, StorageUsedinByte= 100, totalAdministrators=2, SubscriptionName= "Business"};
            PlanBusiness planBusiness2 = new PlanBusiness { Id = -2, IsActive=true, CompanyId= listOfCompanyCreated[0].Id, Price=2120000, ExpiredAt=DateTime.Now.AddDays(60), StartedAt=DateTime.Now, StorageUsedinByte= 1000, totalAdministrators=1, SubscriptionName= "Business"};
            PlanBusiness planBusiness3 = new PlanBusiness { Id = -3, IsActive=true, CompanyId= listOfCompanyCreated[2].Id, Price=2103000, ExpiredAt=DateTime.Now.AddDays(50), StartedAt=DateTime.Now, StorageUsedinByte= 10000, totalAdministrators=3, SubscriptionName= "Corporate"};

            Contact contact1 = new Contact { ContactOwnerId = listOfUserCreated[0].Id, ContactItemId = listOfUserCreated[1].Id };
            Contact contact2 = new Contact { ContactOwnerId = listOfUserCreated[0].Id, ContactItemId = listOfUserCreated[2].Id };
            Contact contact3 = new Contact { ContactOwnerId = listOfUserCreated[1].Id, ContactItemId = listOfUserCreated[3].Id };
            Contact contact4 = new Contact { ContactOwnerId = listOfUserCreated[1].Id, ContactItemId = listOfUserCreated[4].Id };
            Contact contact5 = new Contact { ContactOwnerId = listOfUserCreated[1].Id, ContactItemId = listOfUserCreated[5].Id };
            Contact contact6 = new Contact { ContactOwnerId = listOfUserCreated[5].Id, ContactItemId = listOfUserCreated[6].Id };

            Workflow wf1 = new Workflow { Id = -1, CreatorId = listOfUserCreated[0].Id, DateCreated = DateTime.Now, IsActive = true, Name = "myone", Description = "cuy", IsTemplate = false, UserEmail = listOfUserCreated[0].Email };
            
            WorkflowNode wfn1 = new WorkflowNode { Id = -1, WorkflowId = wf1.Id, SymbolCode = 0, Caption = "Start", WorkflowNodeLinkTos = new List<WorkflowNodeLink>(), WorkflowNodeLinks = new List<WorkflowNodeLink>() };
            WorkflowNode wfn2 = new WorkflowNode { Id = -2, WorkflowId = wf1.Id, SymbolCode = 1, Caption = "End", WorkflowNodeLinkTos = new List<WorkflowNodeLink>(), WorkflowNodeLinks = new List<WorkflowNodeLink>() };
            WorkflowNode wfn3 = new WorkflowNode { Id = -3, WorkflowId = wf1.Id, SymbolCode = 5, Caption = "Activity", WorkflowNodeLinkTos = new List<WorkflowNodeLink>(), WorkflowNodeLinks = new List<WorkflowNodeLink>() };

            WorkflowNodeLink wflnl1 = new WorkflowNodeLink { Id = -1, WorkflowNodeId = wfn1.Id , WorkflowNodeToId = wfn3.Id};
            WorkflowNodeLink wflnl2 = new WorkflowNodeLink { Id = -2, WorkflowNodeId = wfn3.Id, WorkflowNodeToId = wfn2.Id };

            modelBuilder.Entity<Workflow>().HasData(wf1);
            modelBuilder.Entity<WorkflowNode>().HasData(wfn1, wfn2, wfn3);
            modelBuilder.Entity<WorkflowNodeLink>().HasData(wflnl1, wflnl2);

            modelBuilder.Entity<Company>().HasData(listOfCompanyCreated[0], listOfCompanyCreated[1], listOfCompanyCreated[2]);
            modelBuilder.Entity<Member>().HasData(member1, member2, member3, member4, member5, member6, member7, member8, member9, member10, member11, member12);
            modelBuilder.Entity<PlanBusiness>().HasData(planBusiness1, planBusiness2, planBusiness3);

            modelBuilder.Entity<Contact>().HasKey(c => new { c.ContactOwnerId, c.ContactItemId });
            modelBuilder.Entity<Contact>().HasData(contact1, contact2, contact3, contact4, contact5, contact6);

            Rotation rt1 = new Rotation { Id = -1, CreatorId = listOfUserCreated[0].Id, Remark = "fe", Status = 1, WorkflowId = wf1.Id,
                Subject = "Goodddd", DateCreated = DateTime.Now, UserId = listOfUserCreated[0].Id};

            RotationNode rtn1 = new RotationNode { Id = -1, UserId = listOfUserCreated[2].Id, RotationId=rt1.Id,
                WorkflowNodeId = wfn3.Id, CreatedAt = DateTime.Now};

            RotationUser rtnusr1 = new RotationUser { Id = -1, FlagPermission = 6, WorkflowNodeId = wfn3.Id, UserId = listOfUserCreated[1].Id, RotationId = rt1.Id };

            Inbox inbox1 = new Inbox { Id = -1, UserId = rtn1.UserId, ActivityId = rtn1.Id, 
                Message = listOfUserCreated[2].Name + ", you has new Work on Rotatiion " + rt1.Subject, IsUnread = true, CreatedAt = DateTime.Now, 
                DateNote = "New Created Inbox from " + rt1.Subject};

            rt1.DateUpdated = DateTime.Now;
            rt1.CompanyId = listOfCompanyCreated[2].Id;
            rt1.DateStarted = DateTime.Now;

            modelBuilder.Entity<Rotation>().HasData(rt1);
            modelBuilder.Entity<RotationNode>().HasData(rtn1);
            modelBuilder.Entity<RotationUser>().HasData(rtnusr1);
            modelBuilder.Entity<Inbox>().HasData(inbox1);
        }
    }
}
