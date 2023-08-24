using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class HostelStaffandexeatupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoomNo",
                table: "HostelRoomDetails",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "GrantedBy",
                table: "Exeats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GrantedOn",
                table: "Exeats",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "HostelStaffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    HostelId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AdminType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostelStaffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelStaffs_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostelStaffs_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Exeats_GrantedBy",
                table: "Exeats",
                column: "GrantedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HostelStaffs_HostelId",
                table: "HostelStaffs",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelStaffs_StaffId",
                table: "HostelStaffs",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exeats_Staffs_GrantedBy",
                table: "Exeats",
                column: "GrantedBy",
                principalTable: "Staffs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exeats_Staffs_GrantedBy",
                table: "Exeats");

            migrationBuilder.DropTable(
                name: "HostelStaffs");

            migrationBuilder.DropIndex(
                name: "IX_Exeats_GrantedBy",
                table: "Exeats");

            migrationBuilder.DropColumn(
                name: "GrantedBy",
                table: "Exeats");

            migrationBuilder.DropColumn(
                name: "GrantedOn",
                table: "Exeats");

            migrationBuilder.AlterColumn<int>(
                name: "RoomNo",
                table: "HostelRoomDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
