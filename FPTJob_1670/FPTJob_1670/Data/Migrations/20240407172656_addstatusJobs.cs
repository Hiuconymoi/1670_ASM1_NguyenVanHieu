using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTJob_1670.Data.Migrations
{
    /// <inheritdoc />
    public partial class addstatusJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "statusJobs",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "statusJobs",
                table: "Job");
        }
    }
}
