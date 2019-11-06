using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    public partial class AddDocumentElementWithRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentElements",
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
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentElements_ElementTypes_ElementTypeId",
                        column: x => x.ElementTypeId,
                        principalTable: "ElementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentElements_DocumentId",
                table: "DocumentElements",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentElements_ElementTypeId",
                table: "DocumentElements",
                column: "ElementTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentElements");
        }
    }
}
