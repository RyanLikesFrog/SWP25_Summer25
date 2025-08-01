using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_TreatmentStages_TreatmentStageId",
                table: "PrescriptionItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_TreatmentStages_TreatmentStageId",
                table: "PrescriptionItems",
                column: "TreatmentStageId",
                principalTable: "TreatmentStages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_TreatmentStages_TreatmentStageId",
                table: "PrescriptionItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_TreatmentStages_TreatmentStageId",
                table: "PrescriptionItems",
                column: "TreatmentStageId",
                principalTable: "TreatmentStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
