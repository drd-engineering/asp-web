using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using DRD.Models;

namespace DRD.Service.Models
{
    public class MyDBContext : System.Based.Core.Entity.DrdContext
    {
        public MyDBContext()
        {

        }
        public DbSet<AnnotateType> AnnotateType { get; set; } 
        public DbSet<Company> Company { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DocumentElement> DocumentElement { get; set; }
        public DbSet<DocumentSign> DocumentSign { get; set; }
        public DbSet<FaspayData> FaspayData { get; set; }
        public DbSet<Member> Member { get; set; }
         public DbSet<Plan> Plan { get; set; }
        public DbSet<Rotation> Rotation { get; set; }
        public DbSet<RotationMember> RotationMember { get; set; }
        public DbSet<RotationNode> RotationNode { get; set; }
        public DbSet<RotationNodeDoc> RotationNodeDoc { get; set; }
        public DbSet<RotationNodeRemark> RotationNodeRemark { get; set; }
        public DbSet<RotationNodeUpDoc> RotationNodeUpDoc { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserAdmin> UserAdmin { get; set; }
        public DbSet<Workflow> Workflow { get; set; }
        public DbSet<WorkflowNode> WorkflowNode { get; set; }
        public DbSet<WorkflowNodeLink> WorkflowNodeLink { get; set; }



    }
}