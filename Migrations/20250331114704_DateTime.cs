using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestSignalR.Migrations
{
    /// <inheritdoc />
    public partial class DateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "RegistredAt",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "SendedAt",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistredAt",
                table: "Users",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SendedAt",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");
        }
    }
}
