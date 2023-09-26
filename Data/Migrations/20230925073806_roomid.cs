using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class roomid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoomIdId",
                table: "HostelAllocations",
                newName: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocations_RoomId",
                table: "HostelAllocations",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_HostelAllocations_HostelRoomDetails_RoomId",
                table: "HostelAllocations",
                column: "RoomId",
                principalTable: "HostelRoomDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HostelAllocations_HostelRoomDetails_RoomId",
                table: "HostelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_HostelAllocations_RoomId",
                table: "HostelAllocations");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "HostelAllocations",
                newName: "RoomIdId");
        }
    }
}
