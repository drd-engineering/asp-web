using DRD.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DRD.Service.Context
{
    public class Connection : DbContext
    {
        public Connection() :   base(Constant.CONSTRING)
        { 
        }

        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<BusinessUsage> BusinessUsages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentAnnotation> DocumentElements { get; set; }
        public DbSet<DocumentUser> DocumentUsers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BusinessPackage> BusinessPackages { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<RotationUser> RotationUsers { get; set; }
        public DbSet<RotationNode> RotationNodes { get; set; }
        public DbSet<RotationNodeDoc> RotationNodeDocs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Inbox> Inboxes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workflow> Workflows { get; set; }

        public DbSet<WorkflowNode> WorkflowNodes { get; set; }
        public DbSet<WorkflowNodeLink> WorkflowNodeLinks { get; set; }
        public DbSet<TagItem> TagItems { get; set; }
        public DbSet<Price> Prices { get; set; }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
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
    }
}