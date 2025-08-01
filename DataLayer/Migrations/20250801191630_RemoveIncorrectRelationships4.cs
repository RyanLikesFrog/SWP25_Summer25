using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIncorrectRelationships4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_TreatmentStages_TreatmentStageId",
                table: "PrescriptionItems");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItems_TreatmentStageId",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "TreatmentStageId",
                table: "PrescriptionItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TreatmentStageId",
                table: "PrescriptionItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_TreatmentStageId",
                table: "PrescriptionItems",
                column: "TreatmentStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_TreatmentStages_TreatmentStageId",
                table: "PrescriptionItems",
                column: "TreatmentStageId",
                principalTable: "TreatmentStages",
                principalColumn: "Id");
        }
    }
}
