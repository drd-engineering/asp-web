using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Core.Postgres.Migrations
{
    public partial class AddRotationMemberAndRotationRemark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RotationMember",
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
                    RotationId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationMember_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotationMember_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RotationNodeRemark",
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
                    table.PrimaryKey("PK_RotationNodeRemark", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationNodeRemark_RotationNode_RotationNodeId",
                        column: x => x.RotationNodeId,
                        principalTable: "RotationNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RotationMember_MemberId",
                table: "RotationMember",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationMember_RotationId",
                table: "RotationMember",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RotationNodeRemark_RotationNodeId",
                table: "RotationNodeRemark",
                column: "RotationNodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RotationMember");

            migrationBuilder.DropTable(
                name: "RotationNodeRemark");
        }
    }
}
