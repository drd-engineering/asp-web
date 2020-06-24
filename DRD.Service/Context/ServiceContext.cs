using DRD.Models;
using System.Data.Entity;

namespace DRD.Service.Context
{
    public class ServiceContext : DbContext
    {
        public ServiceContext() : base("AppContext")
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<BusinessUsage> Usages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentElement> DocumentElements { get; set; }
        public DbSet<DocumentUser> DocumentUsers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BusinessPackage> BusinessPackages { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<RotationUser> RotationUsers { get; set; }
        public DbSet<RotationNode> RotationNodes { get; set; }
        public DbSet<RotationNodeDoc> RotationNodeDocs { get; set; }
        public DbSet<RotationNodeRemark> RotationNodeRemarks { get; set; }
        public DbSet<RotationNodeUpDoc> RotationNodeUpDocs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Inbox> Inboxes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Stamp> Stamps { get; set; }

        // public DbSet<Symbol> Symbols{ get; set; }
        public DbSet<Workflow> Workflows { get; set; }

        public DbSet<WorkflowNode> WorkflowNodes { get; set; }
        public DbSet<WorkflowNodeLink> WorkflowNodeLinks { get; set; }
        public DbSet<TagItem> TagItems { get; set; }
        public DbSet<Price> Prices { get; set; }
    }
}