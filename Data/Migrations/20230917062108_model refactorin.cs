using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class modelrefactorin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversionMainWallets_ConversionApplicants_ApplicantId",
                table: "ConversionMainWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversionSubWallets_ConversionApplicants_ApplicantId",
                table: "ConversionSubWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_HostelAllocations_Students_StudentId",
                table: "HostelAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PgMainWallets_PgApplicants_ApplicantId",
                table: "PgMainWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_PgSubWallets_UgApplicants_ApplicantId",
                table: "PgSubWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_UgMainWallets_UgApplicants_ApplicantId",
                table: "UgMainWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_UgSubWallets_UgApplicants_ApplicantId",
                table: "UgSubWallets");

            migrationBuilder.DropIndex(
                name: "IX_UgSubWallets_ApplicantId",
                table: "UgSubWallets");

            migrationBuilder.DropIndex(
                name: "IX_UgMainWallets_ApplicantId",
                table: "UgMainWallets");

            migrationBuilder.DropIndex(
                name: "IX_PgSubWallets_ApplicantId",
                table: "PgSubWallets");

            migrationBuilder.DropIndex(
                name: "IX_PgMainWallets_ApplicantId",
                table: "PgMainWallets");

            migrationBuilder.DropIndex(
                name: "IX_ConversionSubWallets_ApplicantId",
                table: "ConversionSubWallets");

            migrationBuilder.DropIndex(
                name: "IX_ConversionMainWallets_ApplicantId",
                table: "ConversionMainWallets");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "UgSubWallets");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "PgSubWallets");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "PgMainWallets");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "ConversionSubWallets");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "ConversionMainWallets");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "HostelAllocations",
                newName: "WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_HostelAllocations_StudentId",
                table: "HostelAllocations",
                newName: "IX_HostelAllocations_WalletId");

            migrationBuilder.AddColumn<int>(
                name: "StudentType",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HostelAllocations_UgMainWallets_WalletId",
                table: "HostelAllocations",
                column: "WalletId",
                principalTable: "UgMainWallets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HostelAllocations_UgMainWallets_WalletId",
                table: "HostelAllocations");

            migrationBuilder.DropColumn(
                name: "StudentType",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "HostelAllocations",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_HostelAllocations_WalletId",
                table: "HostelAllocations",
                newName: "IX_HostelAllocations_StudentId");

            migrationBuilder.AddColumn<int>(
                name: "ApplicantId",
                table: "UgSubWallets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantId",
                table: "PgSubWallets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantId",
                table: "PgMainWallets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantId",
                table: "ConversionSubWallets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantId",
                table: "ConversionMainWallets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UgSubWallets_ApplicantId",
                table: "UgSubWallets",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_UgMainWallets_ApplicantId",
                table: "UgMainWallets",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_PgSubWallets_ApplicantId",
                table: "PgSubWallets",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_PgMainWallets_ApplicantId",
                table: "PgMainWallets",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionSubWallets_ApplicantId",
                table: "ConversionSubWallets",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionMainWallets_ApplicantId",
                table: "ConversionMainWallets",
                column: "ApplicantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversionMainWallets_ConversionApplicants_ApplicantId",
                table: "ConversionMainWallets",
                column: "ApplicantId",
                principalTable: "ConversionApplicants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversionSubWallets_ConversionApplicants_ApplicantId",
                table: "ConversionSubWallets",
                column: "ApplicantId",
                principalTable: "ConversionApplicants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HostelAllocations_Students_StudentId",
                table: "HostelAllocations",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PgMainWallets_PgApplicants_ApplicantId",
                table: "PgMainWallets",
                column: "ApplicantId",
                principalTable: "PgApplicants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_PgSubWallets_UgApplicants_ApplicantId",
                table: "PgSubWallets",
                column: "ApplicantId",
                principalTable: "UgApplicants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UgMainWallets_UgApplicants_ApplicantId",
                table: "UgMainWallets",
                column: "ApplicantId",
                principalTable: "UgApplicants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UgSubWallets_UgApplicants_ApplicantId",
                table: "UgSubWallets",
                column: "ApplicantId",
                principalTable: "UgApplicants",
                principalColumn: "Id");
        }
    }
}
