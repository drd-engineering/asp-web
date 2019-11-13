using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Descr = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    PointLocation = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Image1 = table.Column<string>(nullable: true),
                    Image2 = table.Column<string>(nullable: true),
                    ImageCard = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElementTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<long>(nullable: false),
                    SubscriptTypeId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PriceUnitCode = table.Column<string>(nullable: true),
                    PriceUnitDescr = table.Column<string>(nullable: true),
                    RotationCount = table.Column<int>(nullable: false),
                    RotationPrice = table.Column<decimal>(nullable: false),
                    FlowActivityCount = table.Column<int>(nullable: false),
                    FlowActivityPrice = table.Column<decimal>(nullable: false),
                    StorageSize = table.Column<long>(nullable: false),
                    StoragePrice = table.Column<decimal>(nullable: false),
                    DrDriveSize = table.Column<long>(nullable: false),
                    DrDrivePrice = table.Column<decimal>(nullable: false),
                    ExpiryDocDay = table.Column<int>(nullable: false),
                    PackageExpiryDay = table.Column<int>(nullable: false),
                    DrDriveExpiryDay = table.Column<int>(nullable: false),
                    ValidPackage = table.Column<DateTime>(nullable: true),
                    ValidDrDrive = table.Column<DateTime>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Symbols",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    SymbolType = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symbols", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAdmins",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    AdminType = table.Column<int>(nullable: false),
                    AppZoneAccess = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    PanelType = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdmins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    OfficialIdNo = table.Column<long>(nullable: false),
                    ImageProfile = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    ImageSignature = table.Column<string>(nullable: true),
                    ImageInitials = table.Column<string>(nullable: true),
                    ImageStamp = table.Column<string>(nullable: true),
                    ImageKtp1 = table.Column<string>(nullable: true),
                    ImageKtp2 = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatorId = table.Column<long>(nullable: true),
                    IsTemplate = table.Column<bool>(nullable: false),
                    WfType = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(nullable: true),
                    Descr = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileSize = table.Column<int>(nullable: false),
                    CreatorId = table.Column<long>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CompaniesId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Companies_CompaniesId",
                        column: x => x.CompaniesId,
                        principalSchema: "public",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<long>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    JoinedAt = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsAdministrator = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "public",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    CompaniesId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Companies_CompaniesId",
                        column: x => x.CompaniesId,
                        principalSchema: "public",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentElements",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Page = table.Column<int>(nullable: false),
                    LeftPos = table.Column<double>(nullable: true),
                    TopPos = table.Column<double>(nullable: true),
                    WidthPos = table.Column<double>(nullable: true),
                    HeightPos = table.Column<double>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    BackColor = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    Data2 = table.Column<string>(nullable: true),
                    Rotation = table.Column<int>(nullable: false),
                    ScaleX = table.Column<double>(nullable: false),
                    ScaleY = table.Column<double>(nullable: false),
                    TransX = table.Column<double>(nullable: false),
                    TransY = table.Column<double>(nullable: false),
                    StrokeWidth = table.Column<double>(nullable: false),
                    Opacity = table.Column<double>(nullable: false),
                    CreatorId = table.Column<long>(nullable: true),
                    AnnotateId = table.Column<long>(nullable: true),
                    Flag = table.Column<int>(nullable: false),
                    FlagCode = table.Column<string>(nullable: true),
                    FlagDate = table.Column<DateTime>(nullable: true),
                    FlagImage = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ElementTypeId = table.Column<int>(nullable: true),
                    DocumentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentElements_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "public",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentElements_ElementTypes_ElementTypeId",
                        column: x => x.ElementTypeId,
                        principalSchema: "public",
                        principalTable: "ElementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rotations",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Subject = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreatorId = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateStarted = table.Column<DateTime>(nullable: true),
                    StatusDescr = table.Column<string>(nullable: true),
                    RotationNodeId = table.Column<long>(nullable: false),
                    DefWorkflowNodeId = table.Column<long>(nullable: false),
                    FlagAction = table.Column<int>(nullable: false),
                    DecissionInfo = table.Column<string>(nullable: true),
                    MemberId = table.Column<long>(nullable: true),
                    WorkflowId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rotations_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "public",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rotations_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "public",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowNodes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowId = table.Column<long>(nullable: false),
                    MemberId = table.Column<long>(nullable: true),
                    SymbolId = table.Column<int>(nullable: false),
                    Caption = table.Column<string>(nullable: true),
                    Info = table.Column<string>(nullable: true),
                    Operator = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    PosLeft = table.Column<string>(nullable: true),
                    PosTop = table.Column<string>(nullable: true),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    TextColor = table.Column<string>(nullable: true),
                    BackColor = table.Column<string>(nullable: true),
                    Flag = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowNodes_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "public",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowNodes_Symbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "public",
                        principalTable: "Symbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowNodes_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "public",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RotationMembers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlagPermission = table.Column<int>(nullable: false),
                    ActivityName = table.Column<string>(nullable: true),
                    MemberPicture = table.Column<string>(nullable: true),
                    MemberNumber = table.Column<string>(nullable: true),
                    MemberName = table.Column<string>(nullable: true),
                    MemberEmail = table.Column<string>(nullable: true),
                    MemberId = table.Column<long>(nullable: true),
                    RotationId = table.Column<long>(nullable: true),
                    WorkflowNodeId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "public",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationMembers_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalSchema: "public",
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationMembers_WorkflowNodes_WorkflowNodeId",
                        column: x => x.WorkflowNodeId,
                        principalSchema: "public",
                        principalTable: "WorkflowNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RotationNodes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrevWorkflowNodeId = table.Column<long>(nullable: true),
                    SenderRotationNodeId = table.Column<long>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    DateRead = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    MemberId = table.Column<long>(nullable: true),
                    RotationId = table.Column<long>(nullable: true),
                    WorkflowNodeId = table.Column<long>(nullable: true),
                    RotationNode_SenderRotationNodeIdId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationNodes_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "public",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNodes_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalSchema: "public",
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNodes_RotationNodes_RotationNode_SenderRotationNode~",
                        column: x => x.RotationNode_SenderRotationNodeIdId,
                        principalSchema: "public",
                        principalTable: "RotationNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNodes_WorkflowNodes_WorkflowNodeId",
                        column: x => x.WorkflowNodeId,
                        principalSchema: "public",
                        principalTable: "WorkflowNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowNodeLinks",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowNodeId = table.Column<long>(nullable: false),
                    WorkflowNodeToId = table.Column<long>(nullable: false),
                    Caption = table.Column<string>(nullable: true),
                    Operator = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    SymbolId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowNodeLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowNodeLinks_Symbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "public",
                        principalTable: "Symbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowNodeLinks_WorkflowNodes_WorkflowNodeId",
                        column: x => x.WorkflowNodeId,
                        principalSchema: "public",
                        principalTable: "WorkflowNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RotationNodeDocs",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlagAction = table.Column<int>(nullable: false),
                    DocumentId = table.Column<long>(nullable: true),
                    RotationNodeId = table.Column<long>(nullable: true),
                    RotationId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationNodeDocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationNodeDocs_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "public",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNodeDocs_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalSchema: "public",
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNodeDocs_RotationNodes_RotationNodeId",
                        column: x => x.RotationNodeId,
                        principalSchema: "public",
                        principalTable: "RotationNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RotationNodeRemarks",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RotationNodeId = table.Column<long>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    DateStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationNodeRemarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationNodeRemarks_RotationNodes_RotationNodeId",
                        column: x => x.RotationNodeId,
                        principalSchema: "public",
                        principalTable: "RotationNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RotationNodeUpDocs",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentUploadId = table.Column<long>(nullable: true),
                    RotationNodeId = table.Column<long>(nullable: true),
                    RotationId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationNodeUpDocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationNodeUpDocs_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalSchema: "public",
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNodeUpDocs_RotationNodes_RotationNodeId",
                        column: x => x.RotationNodeId,
                        principalSchema: "public",
                        principalTable: "RotationNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentElements_DocumentId",
                schema: "public",
                table: "DocumentElements",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentElements_ElementTypeId",
                schema: "public",
                table: "DocumentElements",
                column: "ElementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CompaniesId",
                schema: "public",
                table: "Documents",
                column: "CompaniesId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_CompanyId",
                schema: "public",
                table: "Members",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationMembers_MemberId",
                schema: "public",
                table: "RotationMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationMembers_RotationId",
                schema: "public",
                table: "RotationMembers",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationMembers_WorkflowNodeId",
                schema: "public",
                table: "RotationMembers",
                column: "WorkflowNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeDocs_DocumentId",
                schema: "public",
                table: "RotationNodeDocs",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeDocs_RotationId",
                schema: "public",
                table: "RotationNodeDocs",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeDocs_RotationNodeId",
                schema: "public",
                table: "RotationNodeDocs",
                column: "RotationNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeRemarks_RotationNodeId",
                schema: "public",
                table: "RotationNodeRemarks",
                column: "RotationNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodes_MemberId",
                schema: "public",
                table: "RotationNodes",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodes_RotationId",
                schema: "public",
                table: "RotationNodes",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodes_RotationNode_SenderRotationNodeIdId",
                schema: "public",
                table: "RotationNodes",
                column: "RotationNode_SenderRotationNodeIdId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodes_WorkflowNodeId",
                schema: "public",
                table: "RotationNodes",
                column: "WorkflowNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeUpDocs_RotationId",
                schema: "public",
                table: "RotationNodeUpDocs",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeUpDocs_RotationNodeId",
                schema: "public",
                table: "RotationNodeUpDocs",
                column: "RotationNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotations_MemberId",
                schema: "public",
                table: "Rotations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotations_WorkflowId",
                schema: "public",
                table: "Rotations",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CompaniesId",
                schema: "public",
                table: "Tags",
                column: "CompaniesId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNodeLinks_SymbolId",
                schema: "public",
                table: "WorkflowNodeLinks",
                column: "SymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNodeLinks_WorkflowNodeId",
                schema: "public",
                table: "WorkflowNodeLinks",
                column: "WorkflowNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNodes_MemberId",
                schema: "public",
                table: "WorkflowNodes",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNodes_SymbolId",
                schema: "public",
                table: "WorkflowNodes",
                column: "SymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNodes_WorkflowId",
                schema: "public",
                table: "WorkflowNodes",
                column: "WorkflowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentElements",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RotationMembers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RotationNodeDocs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RotationNodeRemarks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RotationNodeUpDocs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserAdmins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WorkflowNodeLinks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ElementTypes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RotationNodes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Rotations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WorkflowNodes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Members",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Symbols",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Workflows",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "public");
        }
    }
}
