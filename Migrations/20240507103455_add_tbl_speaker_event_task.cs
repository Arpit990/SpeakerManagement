using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakerManagement.Migrations
{
    /// <inheritdoc />
    public partial class add_tbl_speaker_event_task : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                schema: "dbo",
                table: "SpeakerEvents");

            migrationBuilder.RenameColumn(
                name: "EventTaskId",
                schema: "dbo",
                table: "SpeakerEvents",
                newName: "EventId");

            migrationBuilder.CreateTable(
                name: "SpeakerTasks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpeakerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventTaskId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakerTasks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeakerTasks",
                schema: "dbo");

            migrationBuilder.RenameColumn(
                name: "EventId",
                schema: "dbo",
                table: "SpeakerEvents",
                newName: "EventTaskId");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                schema: "dbo",
                table: "SpeakerEvents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
