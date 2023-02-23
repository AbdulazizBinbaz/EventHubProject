using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterDboEventTicket4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_eventTickets_AspNetUsers_UserId",
                table: "eventTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_eventTickets_AspNetUsers_UserId",
                table: "eventTickets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_eventTickets_AspNetUsers_UserId",
                table: "eventTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_eventTickets_AspNetUsers_UserId",
                table: "eventTickets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
