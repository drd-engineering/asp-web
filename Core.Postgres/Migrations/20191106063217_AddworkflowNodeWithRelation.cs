using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    public partial class AddworkflowNodeWithRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WorkflowNodeId",
                table: "RotationNode",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WorkflowNodeId",
                table: "RotationMember",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Symbol",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Descr = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    SymbolType = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symbol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowNode",
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
                    table.PrimaryKey("PK_WorkflowNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowNode_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowNode_Symbol_SymbolId",
                        column: x => x.SymbolId,
                        principalTable: "Symbol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowNode_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RotationNode_WorkflowNodeId",
                table: "RotationNode",
                column: "WorkflowNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationMember_WorkflowNodeId",
                table: "RotationMember",
                column: "WorkflowNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNode_MemberId",
                table: "WorkflowNode",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNode_SymbolId",
                table: "WorkflowNode",
                column: "SymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNode_WorkflowId",
                table: "WorkflowNode",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_RotationMember_WorkflowNode_WorkflowNodeId",
                table: "RotationMember",
                column: "WorkflowNodeId",
                principalTable: "WorkflowNode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RotationNode_WorkflowNode_WorkflowNodeId",
                table: "RotationNode",
                column: "WorkflowNodeId",
                principalTable: "WorkflowNode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RotationMember_WorkflowNode_WorkflowNodeId",
                table: "RotationMember");

            migrationBuilder.DropForeignKey(
                name: "FK_RotationNode_WorkflowNode_WorkflowNodeId",
                table: "RotationNode");

            migrationBuilder.DropTable(
                name: "WorkflowNode");

            migrationBuilder.DropTable(
                name: "Symbol");

            migrationBuilder.DropIndex(
                name: "IX_RotationNode_WorkflowNodeId",
                table: "RotationNode");

            migrationBuilder.DropIndex(
                name: "IX_RotationMember_WorkflowNodeId",
                table: "RotationMember");

            migrationBuilder.DropColumn(
                name: "WorkflowNodeId",
                table: "RotationNode");

            migrationBuilder.DropColumn(
                name: "WorkflowNodeId",
                table: "RotationMember");
        }
    }
}
