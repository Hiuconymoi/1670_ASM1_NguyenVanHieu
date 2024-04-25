using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTJob_1670.Data.Migrations
{
    /// <inheritdoc />
    public partial class addstatusApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "statusApplication",
                table: "Application",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "statusApplication",
                table: "Application");
        }
    }
}
