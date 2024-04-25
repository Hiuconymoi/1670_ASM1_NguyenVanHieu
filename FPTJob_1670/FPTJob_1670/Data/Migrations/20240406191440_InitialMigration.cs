using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTJob_1670.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Job_JobId",
                table: "Application");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Seeker_SeekerId",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_JobId",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_SeekerId",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "SeekerId",
                table: "Application");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Job_Id",
                table: "Application",
                column: "Job_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Seeker_Id",
                table: "Application",
                column: "Seeker_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Job_Job_Id",
                table: "Application",
                column: "Job_Id",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Seeker_Seeker_Id",
                table: "Application",
                column: "Seeker_Id",
                principalTable: "Seeker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Job_Job_Id",
                table: "Application");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Seeker_Seeker_Id",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_Job_Id",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_Seeker_Id",
                table: "Application");

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Application",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeekerId",
                table: "Application",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Application_JobId",
                table: "Application",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_SeekerId",
                table: "Application",
                column: "SeekerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Job_JobId",
                table: "Application",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Seeker_SeekerId",
                table: "Application",
                column: "SeekerId",
                principalTable: "Seeker",
                principalColumn: "Id");
        }
    }
}
