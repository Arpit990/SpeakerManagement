using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakerManagement.Migrations
{
    /// <inheritdoc />
    public partial class change_col_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "dbo",
                table: "Task",
                newName: "TaskName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskName",
                schema: "dbo",
                table: "Task",
                newName: "Name");
        }
    }
}
