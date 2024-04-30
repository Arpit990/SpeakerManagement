using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakerManagement.Migrations
{
    /// <inheritdoc />
    public partial class update_column_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "dbo",
                table: "Organization",
                newName: "OrganizationName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganizationName",
                schema: "dbo",
                table: "Organization",
                newName: "Name");
        }
    }
}
