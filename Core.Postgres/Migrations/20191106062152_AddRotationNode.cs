using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Postgres.Migrations
{
    public partial class AddRotationNode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RotationId",
                table: "RotationNodeDoc",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeDoc_RotationId",
                table: "RotationNodeDoc",
                column: "RotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RotationNodeDoc_Rotations_RotationId",
                table: "RotationNodeDoc",
                column: "RotationId",
                principalTable: "Rotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RotationNodeDoc_Rotations_RotationId",
                table: "RotationNodeDoc");

            migrationBuilder.DropIndex(
                name: "IX_RotationNodeDoc_RotationId",
                table: "RotationNodeDoc");

            migrationBuilder.DropColumn(
                name: "RotationId",
                table: "RotationNodeDoc");
        }
    }
}
