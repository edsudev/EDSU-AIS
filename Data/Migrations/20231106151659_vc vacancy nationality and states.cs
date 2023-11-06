using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class vcvacancynationalityandstates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VcApplications_Countries_NationalityId",
                table: "VcApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_VcApplications_Lgas_LgaId",
                table: "VcApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_VcApplications_States_StateId",
                table: "VcApplications");

            migrationBuilder.RenameColumn(
                name: "StateId",
                table: "VcApplications",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "NationalityId",
                table: "VcApplications",
                newName: "Nationality");

            migrationBuilder.RenameColumn(
                name: "LgaId",
                table: "VcApplications",
                newName: "Lga");

            migrationBuilder.RenameIndex(
                name: "IX_VcApplications_StateId",
                table: "VcApplications",
                newName: "IX_VcApplications_State");

            migrationBuilder.RenameIndex(
                name: "IX_VcApplications_NationalityId",
                table: "VcApplications",
                newName: "IX_VcApplications_Nationality");

            migrationBuilder.RenameIndex(
                name: "IX_VcApplications_LgaId",
                table: "VcApplications",
                newName: "IX_VcApplications_Lga");

            migrationBuilder.AddForeignKey(
                name: "FK_VcApplications_Countries_Nationality",
                table: "VcApplications",
                column: "Nationality",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VcApplications_Lgas_Lga",
                table: "VcApplications",
                column: "Lga",
                principalTable: "Lgas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VcApplications_States_State",
                table: "VcApplications",
                column: "State",
                principalTable: "States",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VcApplications_Countries_Nationality",
                table: "VcApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_VcApplications_Lgas_Lga",
                table: "VcApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_VcApplications_States_State",
                table: "VcApplications");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "VcApplications",
                newName: "StateId");

            migrationBuilder.RenameColumn(
                name: "Nationality",
                table: "VcApplications",
                newName: "NationalityId");

            migrationBuilder.RenameColumn(
                name: "Lga",
                table: "VcApplications",
                newName: "LgaId");

            migrationBuilder.RenameIndex(
                name: "IX_VcApplications_State",
                table: "VcApplications",
                newName: "IX_VcApplications_StateId");

            migrationBuilder.RenameIndex(
                name: "IX_VcApplications_Nationality",
                table: "VcApplications",
                newName: "IX_VcApplications_NationalityId");

            migrationBuilder.RenameIndex(
                name: "IX_VcApplications_Lga",
                table: "VcApplications",
                newName: "IX_VcApplications_LgaId");

            migrationBuilder.AddForeignKey(
                name: "FK_VcApplications_Countries_NationalityId",
                table: "VcApplications",
                column: "NationalityId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VcApplications_Lgas_LgaId",
                table: "VcApplications",
                column: "LgaId",
                principalTable: "Lgas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VcApplications_States_StateId",
                table: "VcApplications",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id");
        }
    }
}
