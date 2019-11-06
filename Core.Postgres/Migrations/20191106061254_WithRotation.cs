using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    public partial class WithRotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rotations",
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
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rotations_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rotations_MemberId",
                table: "Rotations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotations_WorkflowId",
                table: "Rotations",
                column: "WorkflowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rotations");
        }
    }
}
