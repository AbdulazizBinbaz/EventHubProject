using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddEventManagerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventManagerId",
                table: "events",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_events_EventManagerId",
                table: "events",
                column: "EventManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_events_AspNetUsers_EventManagerId",
                table: "events",
                column: "EventManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_AspNetUsers_EventManagerId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_EventManagerId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "EventManagerId",
                table: "events");
        }
    }
}
