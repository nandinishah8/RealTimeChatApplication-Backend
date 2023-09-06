using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalChatApplication.Migrations
{
    /// <inheritdoc />
    public partial class messagedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "Message",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SeenByUserId",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SeenTimestamp",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seen",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SeenByUserId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SeenTimestamp",
                table: "Message");
        }
    }
}
