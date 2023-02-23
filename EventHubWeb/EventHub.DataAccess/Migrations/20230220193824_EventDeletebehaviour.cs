using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EventDeletebehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_eventTickets_events_EventId",
                table: "eventTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_eventTickets_events_EventId",
                table: "eventTickets",
                column: "EventId",
                principalTable: "events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_eventTickets_events_EventId",
                table: "eventTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_eventTickets_events_EventId",
                table: "eventTickets",
                column: "EventId",
                principalTable: "events",
                principalColumn: "EventId");
        }
    }
}
