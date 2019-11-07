using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    public partial class AddRotationNodeUpDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RotationNodeId",
                table: "RotationNodeDoc",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RotationNode",
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
                    RotationNode_SenderRotationNodeIdId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationNode_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNode_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNode_RotationNode_RotationNode_SenderRotationNodeId~",
                        column: x => x.RotationNode_SenderRotationNodeIdId,
                        principalTable: "RotationNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RotationNodeUpDoc",
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
                    table.PrimaryKey("PK_RotationNodeUpDoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationNodeUpDoc_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationNodeUpDoc_RotationNode_RotationNodeId",
                        column: x => x.RotationNodeId,
                        principalTable: "RotationNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeDoc_RotationNodeId",
                table: "RotationNodeDoc",
                column: "RotationNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNode_MemberId",
                table: "RotationNode",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNode_RotationId",
                table: "RotationNode",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNode_RotationNode_SenderRotationNodeIdId",
                table: "RotationNode",
                column: "RotationNode_SenderRotationNodeIdId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeUpDoc_RotationId",
                table: "RotationNodeUpDoc",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeUpDoc_RotationNodeId",
                table: "RotationNodeUpDoc",
                column: "RotationNodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RotationNodeDoc_RotationNode_RotationNodeId",
                table: "RotationNodeDoc",
                column: "RotationNodeId",
                principalTable: "RotationNode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RotationNodeDoc_RotationNode_RotationNodeId",
                table: "RotationNodeDoc");

            migrationBuilder.DropTable(
                name: "RotationNodeUpDoc");

            migrationBuilder.DropTable(
                name: "RotationNode");

            migrationBuilder.DropIndex(
                name: "IX_RotationNodeDoc_RotationNodeId",
                table: "RotationNodeDoc");

            migrationBuilder.DropColumn(
                name: "RotationNodeId",
                table: "RotationNodeDoc");
        }
    }
}
