using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    public partial class AddWorkflowNodeLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descr",
                table: "Symbol");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Symbol",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkflowNodeLink",
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
                    table.PrimaryKey("PK_WorkflowNodeLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowNodeLink_Symbol_SymbolId",
                        column: x => x.SymbolId,
                        principalTable: "Symbol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowNodeLink_WorkflowNode_WorkflowNodeId",
                        column: x => x.WorkflowNodeId,
                        principalTable: "WorkflowNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNodeLink_SymbolId",
                table: "WorkflowNodeLink",
                column: "SymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNodeLink_WorkflowNodeId",
                table: "WorkflowNodeLink",
                column: "WorkflowNodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowNodeLink");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Symbol");

            migrationBuilder.AddColumn<string>(
                name: "Descr",
                table: "Symbol",
                type: "text",
                nullable: true);
        }
    }
}
