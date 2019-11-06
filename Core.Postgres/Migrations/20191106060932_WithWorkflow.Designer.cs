﻿// <auto-generated />
using System;
using Core.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    [DbContext(typeof(DRDContext))]
    [Migration("20191106060932_WithWorkflow")]
    partial class WithWorkflow
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("Core.Postgres.Models.Document", b =>
                {
                    b.HasOne("Core.Postgres.Models.Company", "Companies")
                        .WithMany()
                        .HasForeignKey("CompaniesId");
                });

            modelBuilder.Entity("Core.Postgres.Models.Member", b =>
                {
                    b.HasOne("Core.Postgres.Models.Company", null)
                        .WithMany("Members")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Core.Postgres.Models.Tag", b =>
                {
                    b.HasOne("Core.Postgres.Models.Company", "Companies")
                        .WithMany("Tags")
                        .HasForeignKey("CompaniesId");
                });
#pragma warning restore 612, 618
        }
    }
}
