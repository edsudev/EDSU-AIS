using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class freshersbursaryclearance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BursaryClearancesFreshers_Students_StudentId",
                table: "BursaryClearancesFreshers");

            //migrationBuilder.RenameColumn(
            //    name: "NumberOfRooms",
            //    table: "Hostels",
            //    newName: "NoOfRooms");

            migrationBuilder.AddColumn<int>(
                name: "BedspacesCount",
                table: "Hostels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Hostels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_BursaryClearancesFreshers_UgApplicants_StudentId",
                table: "BursaryClearancesFreshers",
                column: "StudentId",
                principalTable: "UgApplicants",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BursaryClearancesFreshers_UgApplicants_StudentId",
                table: "BursaryClearancesFreshers");

            migrationBuilder.DropColumn(
                name: "BedspacesCount",
                table: "Hostels");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Hostels");

            //migrationBuilder.RenameColumn(
            //    name: "NoOfRooms",
            //    table: "Hostels",
            //    newName: "NumberOfRooms");

            migrationBuilder.AddForeignKey(
                name: "FK_BursaryClearancesFreshers_Students_StudentId",
                table: "BursaryClearancesFreshers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
