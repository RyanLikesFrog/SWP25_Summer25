using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlForPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureURL",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureURL",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabPicture",
                table: "LabResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7f85377c-d97f-4219-b76c-2ae926013d79"),
                column: "ProfilePictureURL",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureURL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LabPicture",
                table: "LabResults");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureURL",
                table: "Doctors",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
