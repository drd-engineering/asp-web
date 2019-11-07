using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Postgres.Migrations
{
    public partial class AddDocumentSignAndSomeChangesOnAttributeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descr",
                table: "Workflows");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Workflows",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Workflows");

            migrationBuilder.AddColumn<string>(
                name: "Descr",
                table: "Workflows",
                type: "text",
                nullable: true);
        }
    }
}
