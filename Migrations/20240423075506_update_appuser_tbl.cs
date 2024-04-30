using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakerManagement.Migrations
{
    /// <inheritdoc />
    public partial class update_appuser_tbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizaionId",
                schema: "dbo",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                schema: "dbo",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizaionId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Website",
                schema: "dbo",
                table: "User");
        }
    }
}
