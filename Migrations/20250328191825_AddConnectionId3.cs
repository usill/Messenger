using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestSignalR.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionId3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConnectionId",
                table: "Users",
                newName: "Connections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Connections",
                table: "Users",
                newName: "ConnectionId");
        }
    }
}
