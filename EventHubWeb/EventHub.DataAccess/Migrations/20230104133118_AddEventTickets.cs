using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "eventTickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventTickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_eventTickets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_eventTickets_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "EventId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_eventTickets_EventId",
                table: "eventTickets",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_eventTickets_UserId",
                table: "eventTickets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "eventTickets");
        }
    }
}
