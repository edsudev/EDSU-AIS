using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class HostelAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HostelAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HostelId = table.Column<int>(type: "int", nullable: true),
                    RoomIdId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostelAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelAllocations_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostelAllocations_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HostelRoomDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HostelId = table.Column<int>(type: "int", nullable: true),
                    RoomNo = table.Column<int>(type: "int", nullable: true),
                    BedSpaces = table.Column<int>(type: "int", nullable: true),
                    BedSpacesCount = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostelRoomDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelRoomDetails_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocations_HostelId",
                table: "HostelAllocations",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocations_StudentId",
                table: "HostelAllocations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelRoomDetails_HostelId",
                table: "HostelRoomDetails",
                column: "HostelId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HostelAllocations");

            migrationBuilder.DropTable(
                name: "HostelRoomDetails");

           
        }
    }
}
