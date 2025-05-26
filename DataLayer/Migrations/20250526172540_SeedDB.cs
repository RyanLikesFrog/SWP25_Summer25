using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SeedDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Password", "PhoneNumber", "Role", "UpdatedAt", "Username", "isActive" },
                values: new object[] { new Guid("7f85377c-d97f-4219-b76c-2ae926013d79"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@yourdomain.com", "admin", "0123456789", "Admin", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7f85377c-d97f-4219-b76c-2ae926013d79"));

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Users");
        }
    }
}
