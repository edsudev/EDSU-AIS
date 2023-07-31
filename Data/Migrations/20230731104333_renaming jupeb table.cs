using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSU_SYSTEM.Data.Migrations
{
    public partial class renamingjupebtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JupebStudents_JupepApplication_ApplicantId",
                table: "JupebStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_JupepApplication_Countries_NationalityId",
                table: "JupepApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JupepApplication_Departments_AdmittedInto",
                table: "JupepApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JupepApplication_Levels_LevelAdmittedTo",
                table: "JupepApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JupepApplication_Lgas_LGAId",
                table: "JupepApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JupepApplication_Programs_ProgrameId",
                table: "JupepApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JupepApplication_Sessions_YearOfAdmission",
                table: "JupepApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JupepApplication_States_StateOfOriginId",
                table: "JupepApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JupepApplication",
                table: "JupepApplication");

            migrationBuilder.RenameTable(
                name: "JupepApplication",
                newName: "JupebApplicants");

            migrationBuilder.RenameIndex(
                name: "IX_JupepApplication_YearOfAdmission",
                table: "JupebApplicants",
                newName: "IX_JupebApplicants_YearOfAdmission");

            migrationBuilder.RenameIndex(
                name: "IX_JupepApplication_StateOfOriginId",
                table: "JupebApplicants",
                newName: "IX_JupebApplicants_StateOfOriginId");

            migrationBuilder.RenameIndex(
                name: "IX_JupepApplication_ProgrameId",
                table: "JupebApplicants",
                newName: "IX_JupebApplicants_ProgrameId");

            migrationBuilder.RenameIndex(
                name: "IX_JupepApplication_NationalityId",
                table: "JupebApplicants",
                newName: "IX_JupebApplicants_NationalityId");

            migrationBuilder.RenameIndex(
                name: "IX_JupepApplication_LGAId",
                table: "JupebApplicants",
                newName: "IX_JupebApplicants_LGAId");

            migrationBuilder.RenameIndex(
                name: "IX_JupepApplication_LevelAdmittedTo",
                table: "JupebApplicants",
                newName: "IX_JupebApplicants_LevelAdmittedTo");

            migrationBuilder.RenameIndex(
                name: "IX_JupepApplication_AdmittedInto",
                table: "JupebApplicants",
                newName: "IX_JupebApplicants_AdmittedInto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JupebApplicants",
                table: "JupebApplicants",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebApplicants_Countries_NationalityId",
                table: "JupebApplicants",
                column: "NationalityId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebApplicants_Departments_AdmittedInto",
                table: "JupebApplicants",
                column: "AdmittedInto",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebApplicants_Levels_LevelAdmittedTo",
                table: "JupebApplicants",
                column: "LevelAdmittedTo",
                principalTable: "Levels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebApplicants_Lgas_LGAId",
                table: "JupebApplicants",
                column: "LGAId",
                principalTable: "Lgas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebApplicants_Programs_ProgrameId",
                table: "JupebApplicants",
                column: "ProgrameId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebApplicants_Sessions_YearOfAdmission",
                table: "JupebApplicants",
                column: "YearOfAdmission",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebApplicants_States_StateOfOriginId",
                table: "JupebApplicants",
                column: "StateOfOriginId",
                principalTable: "States",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebStudents_JupebApplicants_ApplicantId",
                table: "JupebStudents",
                column: "ApplicantId",
                principalTable: "JupebApplicants",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JupebApplicants_Countries_NationalityId",
                table: "JupebApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JupebApplicants_Departments_AdmittedInto",
                table: "JupebApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JupebApplicants_Levels_LevelAdmittedTo",
                table: "JupebApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JupebApplicants_Lgas_LGAId",
                table: "JupebApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JupebApplicants_Programs_ProgrameId",
                table: "JupebApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JupebApplicants_Sessions_YearOfAdmission",
                table: "JupebApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JupebApplicants_States_StateOfOriginId",
                table: "JupebApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JupebStudents_JupebApplicants_ApplicantId",
                table: "JupebStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JupebApplicants",
                table: "JupebApplicants");

            migrationBuilder.RenameTable(
                name: "JupebApplicants",
                newName: "JupepApplication");

            migrationBuilder.RenameIndex(
                name: "IX_JupebApplicants_YearOfAdmission",
                table: "JupepApplication",
                newName: "IX_JupepApplication_YearOfAdmission");

            migrationBuilder.RenameIndex(
                name: "IX_JupebApplicants_StateOfOriginId",
                table: "JupepApplication",
                newName: "IX_JupepApplication_StateOfOriginId");

            migrationBuilder.RenameIndex(
                name: "IX_JupebApplicants_ProgrameId",
                table: "JupepApplication",
                newName: "IX_JupepApplication_ProgrameId");

            migrationBuilder.RenameIndex(
                name: "IX_JupebApplicants_NationalityId",
                table: "JupepApplication",
                newName: "IX_JupepApplication_NationalityId");

            migrationBuilder.RenameIndex(
                name: "IX_JupebApplicants_LGAId",
                table: "JupepApplication",
                newName: "IX_JupepApplication_LGAId");

            migrationBuilder.RenameIndex(
                name: "IX_JupebApplicants_LevelAdmittedTo",
                table: "JupepApplication",
                newName: "IX_JupepApplication_LevelAdmittedTo");

            migrationBuilder.RenameIndex(
                name: "IX_JupebApplicants_AdmittedInto",
                table: "JupepApplication",
                newName: "IX_JupepApplication_AdmittedInto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JupepApplication",
                table: "JupepApplication",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupebStudents_JupepApplication_ApplicantId",
                table: "JupebStudents",
                column: "ApplicantId",
                principalTable: "JupepApplication",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupepApplication_Countries_NationalityId",
                table: "JupepApplication",
                column: "NationalityId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupepApplication_Departments_AdmittedInto",
                table: "JupepApplication",
                column: "AdmittedInto",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupepApplication_Levels_LevelAdmittedTo",
                table: "JupepApplication",
                column: "LevelAdmittedTo",
                principalTable: "Levels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupepApplication_Lgas_LGAId",
                table: "JupepApplication",
                column: "LGAId",
                principalTable: "Lgas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupepApplication_Programs_ProgrameId",
                table: "JupepApplication",
                column: "ProgrameId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupepApplication_Sessions_YearOfAdmission",
                table: "JupepApplication",
                column: "YearOfAdmission",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JupepApplication_States_StateOfOriginId",
                table: "JupepApplication",
                column: "StateOfOriginId",
                principalTable: "States",
                principalColumn: "Id");
        }
    }
}
