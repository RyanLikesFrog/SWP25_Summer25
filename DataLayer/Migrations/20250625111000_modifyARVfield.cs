using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class modifyARVfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientTreatmentProtocols_ARVProtocols_ProtocolId",
                table: "PatientTreatmentProtocols");

            migrationBuilder.RenameColumn(
                name: "ProtocolId",
                table: "PatientTreatmentProtocols",
                newName: "ARVProtocolId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientTreatmentProtocols_ProtocolId",
                table: "PatientTreatmentProtocols",
                newName: "IX_PatientTreatmentProtocols_ARVProtocolId");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "LabPictures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTreatmentProtocols_ARVProtocols_ARVProtocolId",
                table: "PatientTreatmentProtocols",
                column: "ARVProtocolId",
                principalTable: "ARVProtocols",
                principalColumn: "ProtocolId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientTreatmentProtocols_ARVProtocols_ARVProtocolId",
                table: "PatientTreatmentProtocols");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "LabPictures");

            migrationBuilder.RenameColumn(
                name: "ARVProtocolId",
                table: "PatientTreatmentProtocols",
                newName: "ProtocolId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientTreatmentProtocols_ARVProtocolId",
                table: "PatientTreatmentProtocols",
                newName: "IX_PatientTreatmentProtocols_ProtocolId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTreatmentProtocols_ARVProtocols_ProtocolId",
                table: "PatientTreatmentProtocols",
                column: "ProtocolId",
                principalTable: "ARVProtocols",
                principalColumn: "ProtocolId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
