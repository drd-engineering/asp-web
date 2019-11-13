﻿using DRD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Service.Context
{
    public class ServiceContext : DbContext
    {
        public ServiceContext() : base("AppContext"){ }
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
    }
}