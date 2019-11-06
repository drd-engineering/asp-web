﻿// <auto-generated />
using System;
using Core.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    [DbContext(typeof(DRDContext))]
    partial class DRDContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Core.Postgres.Models.Company", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<string>("Contact")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Descr")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Image1")
                        .HasColumnType("text");

                    b.Property<string>("Image2")
                        .HasColumnType("text");

                    b.Property<string>("ImageCard")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("PointLocation")
                        .HasColumnType("text");

                    b.Property<string>("PostalCode")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Core.Postgres.Models.Document", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("CompaniesId")
                        .HasColumnType("bigint");

                    b.Property<long?>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Descr")
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<int>("FileSize")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CompaniesId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Core.Postgres.Models.DocumentElement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("AnnotateId")
                        .HasColumnType("bigint");

                    b.Property<string>("BackColor")
                        .HasColumnType("text");

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<long?>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.Property<string>("Data2")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("DocumentId")
                        .HasColumnType("bigint");

                    b.Property<int?>("ElementTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("Flag")
                        .HasColumnType("integer");

                    b.Property<string>("FlagCode")
                        .HasColumnType("text");

                    b.Property<DateTime?>("FlagDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FlagImage")
                        .HasColumnType("text");

                    b.Property<double?>("HeightPos")
                        .HasColumnType("double precision");

                    b.Property<double?>("LeftPos")
                        .HasColumnType("double precision");

                    b.Property<double>("Opacity")
                        .HasColumnType("double precision");

                    b.Property<int>("Page")
                        .HasColumnType("integer");

                    b.Property<int>("Rotation")
                        .HasColumnType("integer");

                    b.Property<double>("ScaleX")
                        .HasColumnType("double precision");

                    b.Property<double>("ScaleY")
                        .HasColumnType("double precision");

                    b.Property<double>("StrokeWidth")
                        .HasColumnType("double precision");

                    b.Property<double?>("TopPos")
                        .HasColumnType("double precision");

                    b.Property<double>("TransX")
                        .HasColumnType("double precision");

                    b.Property<double>("TransY")
                        .HasColumnType("double precision");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<double?>("WidthPos")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("ElementTypeId");

                    b.ToTable("DocumentElements");
                });

            modelBuilder.Entity("Core.Postgres.Models.ElementType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ElementTypes");
                });

            modelBuilder.Entity("Core.Postgres.Models.Member", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("CompanyId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("LoginId")
                        .HasColumnType("bigint");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Core.Postgres.Models.Plan", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DrDriveExpiryDay")
                        .HasColumnType("integer");

                    b.Property<decimal>("DrDrivePrice")
                        .HasColumnType("numeric");

                    b.Property<long>("DrDriveSize")
                        .HasColumnType("bigint");

                    b.Property<int>("ExpiryDocDay")
                        .HasColumnType("integer");

                    b.Property<int>("FlowActivityCount")
                        .HasColumnType("integer");

                    b.Property<decimal>("FlowActivityPrice")
                        .HasColumnType("numeric");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<long>("MemberId")
                        .HasColumnType("bigint");

                    b.Property<int>("PackageExpiryDay")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("PriceUnitCode")
                        .HasColumnType("text");

                    b.Property<string>("PriceUnitDescr")
                        .HasColumnType("text");

                    b.Property<int>("RotationCount")
                        .HasColumnType("integer");

                    b.Property<decimal>("RotationPrice")
                        .HasColumnType("numeric");

                    b.Property<decimal>("StoragePrice")
                        .HasColumnType("numeric");

                    b.Property<long>("StorageSize")
                        .HasColumnType("bigint");

                    b.Property<int>("SubscriptTypeId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ValidDrDrive")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ValidPackage")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("Core.Postgres.Models.Rotation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateStarted")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("DecissionInfo")
                        .HasColumnType("text");

                    b.Property<long>("DefWorkflowNodeId")
                        .HasColumnType("bigint");

                    b.Property<int>("FlagAction")
                        .HasColumnType("integer");

                    b.Property<long?>("MemberId")
                        .HasColumnType("bigint");

                    b.Property<string>("Remark")
                        .HasColumnType("text");

                    b.Property<long>("RotationNodeId")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("StatusDescr")
                        .HasColumnType("text");

                    b.Property<string>("Subject")
                        .HasColumnType("text");

                    b.Property<long?>("WorkflowId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("WorkflowId");

                    b.ToTable("Rotations");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationMember", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ActivityName")
                        .HasColumnType("text");

                    b.Property<int>("FlagPermission")
                        .HasColumnType("integer");

                    b.Property<string>("MemberEmail")
                        .HasColumnType("text");

                    b.Property<long?>("MemberId")
                        .HasColumnType("bigint");

                    b.Property<string>("MemberName")
                        .HasColumnType("text");

                    b.Property<string>("MemberNumber")
                        .HasColumnType("text");

                    b.Property<string>("MemberPicture")
                        .HasColumnType("text");

                    b.Property<long?>("RotationId")
                        .HasColumnType("bigint");

                    b.Property<long?>("WorkflowNodeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("RotationId");

                    b.HasIndex("WorkflowNodeId");

                    b.ToTable("RotationMember");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateRead")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("MemberId")
                        .HasColumnType("bigint");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<long?>("PrevWorkflowNodeId")
                        .HasColumnType("bigint");

                    b.Property<long?>("RotationId")
                        .HasColumnType("bigint");

                    b.Property<long?>("RotationNode_SenderRotationNodeIdId")
                        .HasColumnType("bigint");

                    b.Property<long?>("SenderRotationNodeId")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.Property<long?>("WorkflowNodeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("RotationId");

                    b.HasIndex("RotationNode_SenderRotationNodeIdId");

                    b.HasIndex("WorkflowNodeId");

                    b.ToTable("RotationNode");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNodeDoc", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("DocumentId")
                        .HasColumnType("bigint");

                    b.Property<int>("FlagAction")
                        .HasColumnType("integer");

                    b.Property<long?>("RotationId")
                        .HasColumnType("bigint");

                    b.Property<long?>("RotationNodeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("RotationId");

                    b.HasIndex("RotationNodeId");

                    b.ToTable("RotationNodeDoc");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNodeRemark", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateStamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Remark")
                        .HasColumnType("text");

                    b.Property<long>("RotationNodeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RotationNodeId");

                    b.ToTable("RotationNodeRemark");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNodeUpDoc", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("DocumentUploadId")
                        .HasColumnType("bigint");

                    b.Property<long?>("RotationId")
                        .HasColumnType("bigint");

                    b.Property<long?>("RotationNodeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RotationId");

                    b.HasIndex("RotationNodeId");

                    b.ToTable("RotationNodeUpDoc");
                });

            modelBuilder.Entity("Core.Postgres.Models.Symbol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("SymbolType")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Symbol");
                });

            modelBuilder.Entity("Core.Postgres.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("CompaniesId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CompaniesId");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("Core.Postgres.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("ImageInitials")
                        .HasColumnType("text");

                    b.Property<string>("ImageKtp1")
                        .HasColumnType("text");

                    b.Property<string>("ImageKtp2")
                        .HasColumnType("text");

                    b.Property<string>("ImageProfile")
                        .HasColumnType("text");

                    b.Property<string>("ImageSignature")
                        .HasColumnType("text");

                    b.Property<string>("ImageStamp")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Core.Postgres.Models.UserAdmin", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AdminType")
                        .HasColumnType("integer");

                    b.Property<string>("AppZoneAccess")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("PanelType")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserAdmins");
                });

            modelBuilder.Entity("Core.Postgres.Models.Workflow", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Descr")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsTemplate")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<int>("WfType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Workflows");
                });

            modelBuilder.Entity("Core.Postgres.Models.WorkflowNode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("BackColor")
                        .HasColumnType("text");

                    b.Property<string>("Caption")
                        .HasColumnType("text");

                    b.Property<int>("Flag")
                        .HasColumnType("integer");

                    b.Property<string>("Height")
                        .HasColumnType("text");

                    b.Property<string>("Info")
                        .HasColumnType("text");

                    b.Property<long?>("MemberId")
                        .HasColumnType("bigint");

                    b.Property<string>("Operator")
                        .HasColumnType("text");

                    b.Property<string>("PosLeft")
                        .HasColumnType("text");

                    b.Property<string>("PosTop")
                        .HasColumnType("text");

                    b.Property<int>("SymbolId")
                        .HasColumnType("integer");

                    b.Property<string>("TextColor")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.Property<string>("Width")
                        .HasColumnType("text");

                    b.Property<long>("WorkflowId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("SymbolId");

                    b.HasIndex("WorkflowId");

                    b.ToTable("WorkflowNode");
                });

            modelBuilder.Entity("Core.Postgres.Models.WorkflowNodeLink", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Caption")
                        .HasColumnType("text");

                    b.Property<string>("Operator")
                        .HasColumnType("text");

                    b.Property<int>("SymbolId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.Property<long>("WorkflowNodeId")
                        .HasColumnType("bigint");

                    b.Property<long>("WorkflowNodeToId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SymbolId");

                    b.HasIndex("WorkflowNodeId");

                    b.ToTable("WorkflowNodeLink");
                });

            modelBuilder.Entity("Core.Postgres.Models.Document", b =>
                {
                    b.HasOne("Core.Postgres.Models.Company", "Companies")
                        .WithMany()
                        .HasForeignKey("CompaniesId");
                });

            modelBuilder.Entity("Core.Postgres.Models.DocumentElement", b =>
                {
                    b.HasOne("Core.Postgres.Models.Document", "Document")
                        .WithMany("DocumentElements")
                        .HasForeignKey("DocumentId");

                    b.HasOne("Core.Postgres.Models.ElementType", "ElementType")
                        .WithMany()
                        .HasForeignKey("ElementTypeId");
                });

            modelBuilder.Entity("Core.Postgres.Models.Member", b =>
                {
                    b.HasOne("Core.Postgres.Models.Company", null)
                        .WithMany("Members")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Core.Postgres.Models.Rotation", b =>
                {
                    b.HasOne("Core.Postgres.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("Core.Postgres.Models.Workflow", "Workflow")
                        .WithMany()
                        .HasForeignKey("WorkflowId");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationMember", b =>
                {
                    b.HasOne("Core.Postgres.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("Core.Postgres.Models.Rotation", "Rotation")
                        .WithMany("RotationMembers")
                        .HasForeignKey("RotationId");

                    b.HasOne("Core.Postgres.Models.WorkflowNode", "WorkflowNode")
                        .WithMany()
                        .HasForeignKey("WorkflowNodeId");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNode", b =>
                {
                    b.HasOne("Core.Postgres.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("Core.Postgres.Models.Rotation", "Rotation")
                        .WithMany("RotationNodes")
                        .HasForeignKey("RotationId");

                    b.HasOne("Core.Postgres.Models.RotationNode", "RotationNode_SenderRotationNodeId")
                        .WithMany("RotationNodes")
                        .HasForeignKey("RotationNode_SenderRotationNodeIdId");

                    b.HasOne("Core.Postgres.Models.WorkflowNode", "WorkflowNode")
                        .WithMany("Rotations")
                        .HasForeignKey("WorkflowNodeId");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNodeDoc", b =>
                {
                    b.HasOne("Core.Postgres.Models.Document", "Document")
                        .WithMany("RotationNodeDocs")
                        .HasForeignKey("DocumentId");

                    b.HasOne("Core.Postgres.Models.Rotation", null)
                        .WithMany("SumRotationNodeDocs")
                        .HasForeignKey("RotationId");

                    b.HasOne("Core.Postgres.Models.RotationNode", "RotationNode")
                        .WithMany("RotationNodeDocs")
                        .HasForeignKey("RotationNodeId");
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNodeRemark", b =>
                {
                    b.HasOne("Core.Postgres.Models.RotationNode", "RotationNode")
                        .WithMany("RotationNodeRemarks")
                        .HasForeignKey("RotationNodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Postgres.Models.RotationNodeUpDoc", b =>
                {
                    b.HasOne("Core.Postgres.Models.Rotation", null)
                        .WithMany("SumRotationNodeUpDocs")
                        .HasForeignKey("RotationId");

                    b.HasOne("Core.Postgres.Models.RotationNode", "RotationNode")
                        .WithMany("RotationNodeUpDocs")
                        .HasForeignKey("RotationNodeId");
                });

            modelBuilder.Entity("Core.Postgres.Models.Tag", b =>
                {
                    b.HasOne("Core.Postgres.Models.Company", "Companies")
                        .WithMany("Tags")
                        .HasForeignKey("CompaniesId");
                });

            modelBuilder.Entity("Core.Postgres.Models.WorkflowNode", b =>
                {
                    b.HasOne("Core.Postgres.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("Core.Postgres.Models.Symbol", "Symbol")
                        .WithMany("WorkflowNodes")
                        .HasForeignKey("SymbolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Postgres.Models.Workflow", "Workflow")
                        .WithMany("WorkflowNodes")
                        .HasForeignKey("WorkflowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Postgres.Models.WorkflowNodeLink", b =>
                {
                    b.HasOne("Core.Postgres.Models.Symbol", "Symbol")
                        .WithMany("WorkflowNodeLinks")
                        .HasForeignKey("SymbolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Postgres.Models.WorkflowNode", "WorkflowNode_WorkflowNodeId")
                        .WithMany("WorkflowNodeLinks_WorkflowNodeId")
                        .HasForeignKey("WorkflowNodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
