using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakerManagement.Migrations
{
    /// <inheritdoc />
    public partial class update_col_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganizaionId",
                schema: "dbo",
                table: "User",
                newName: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "dbo",
                table: "User",
                newName: "OrganizaionId");
        }
    }
}
