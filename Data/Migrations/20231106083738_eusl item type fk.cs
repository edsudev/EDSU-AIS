using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class euslitemtypefk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "EuslPs",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EuslPs_Type",
                table: "EuslPs",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_EuslPs_StudentManuals_Type",
                table: "EuslPs",
                column: "Type",
                principalTable: "StudentManuals",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EuslPs_StudentManuals_Type",
                table: "EuslPs");

            migrationBuilder.DropIndex(
                name: "IX_EuslPs_Type",
                table: "EuslPs");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "EuslPs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
