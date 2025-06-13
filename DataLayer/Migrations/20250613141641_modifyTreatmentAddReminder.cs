using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class modifyTreatmentAddReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderFrequency",
                table: "TreatmentStages");

            migrationBuilder.DropColumn(
                name: "ReferenceRange",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "LabResults");

            migrationBuilder.RenameColumn(
                name: "CustomProtocolDetails",
                table: "TreatmentStages",
                newName: "Medicine");

            migrationBuilder.RenameColumn(
                name: "ResultValue",
                table: "LabResults",
                newName: "ResultSummary");

            migrationBuilder.AddColumn<int>(
                name: "StageNumber",
                table: "TreatmentStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TreatmentStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PatientTreatmentProtocols",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentName",
                table: "PatientTreatmentProtocols",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TestType",
                table: "LabResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Conclusion",
                table: "LabResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "LabResults",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestName",
                table: "LabResults",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_DoctorId",
                table: "LabResults",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabResults_Doctors_DoctorId",
                table: "LabResults",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabResults_Doctors_DoctorId",
                table: "LabResults");

            migrationBuilder.DropIndex(
                name: "IX_LabResults_DoctorId",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "StageNumber",
                table: "TreatmentStages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TreatmentStages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PatientTreatmentProtocols");

            migrationBuilder.DropColumn(
                name: "TreatmentName",
                table: "PatientTreatmentProtocols");

            migrationBuilder.DropColumn(
                name: "Conclusion",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "LabResults");

            migrationBuilder.DropColumn(
                name: "TestName",
                table: "LabResults");

            migrationBuilder.RenameColumn(
                name: "Medicine",
                table: "TreatmentStages",
                newName: "CustomProtocolDetails");

            migrationBuilder.RenameColumn(
                name: "ResultSummary",
                table: "LabResults",
                newName: "ResultValue");

            migrationBuilder.AddColumn<string>(
                name: "ReminderFrequency",
                table: "TreatmentStages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TestType",
                table: "LabResults",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRange",
                table: "LabResults",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "LabResults",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
