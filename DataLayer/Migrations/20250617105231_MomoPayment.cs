using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class MomoPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_PaymentTransactions_PaymentTransactionId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PaymentTransactionId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Gateway",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "TmnCode",
                table: "PaymentTransactions",
                newName: "MomoTransactionId");

            migrationBuilder.RenameColumn(
                name: "ResponseCode",
                table: "PaymentTransactions",
                newName: "MomoSignature");

            migrationBuilder.RenameColumn(
                name: "PaymentGatewayTransactionId",
                table: "PaymentTransactions",
                newName: "MomoResultCode");

            migrationBuilder.RenameColumn(
                name: "PayDate",
                table: "PaymentTransactions",
                newName: "MomoRequestId");

            migrationBuilder.RenameColumn(
                name: "CardType",
                table: "PaymentTransactions",
                newName: "MomoOrderId");

            migrationBuilder.RenameColumn(
                name: "CallbackData",
                table: "PaymentTransactions",
                newName: "MomoMessage");

            migrationBuilder.RenameColumn(
                name: "BankCode",
                table: "PaymentTransactions",
                newName: "MomoExtraData");

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId",
                table: "PaymentTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "MomoResponseTime",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_AppointmentId",
                table: "PaymentTransactions",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Appointments_AppointmentId",
                table: "PaymentTransactions",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Appointments_AppointmentId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_AppointmentId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "MomoResponseTime",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "MomoTransactionId",
                table: "PaymentTransactions",
                newName: "TmnCode");

            migrationBuilder.RenameColumn(
                name: "MomoSignature",
                table: "PaymentTransactions",
                newName: "ResponseCode");

            migrationBuilder.RenameColumn(
                name: "MomoResultCode",
                table: "PaymentTransactions",
                newName: "PaymentGatewayTransactionId");

            migrationBuilder.RenameColumn(
                name: "MomoRequestId",
                table: "PaymentTransactions",
                newName: "PayDate");

            migrationBuilder.RenameColumn(
                name: "MomoOrderId",
                table: "PaymentTransactions",
                newName: "CardType");

            migrationBuilder.RenameColumn(
                name: "MomoMessage",
                table: "PaymentTransactions",
                newName: "CallbackData");

            migrationBuilder.RenameColumn(
                name: "MomoExtraData",
                table: "PaymentTransactions",
                newName: "BankCode");

            migrationBuilder.AddColumn<int>(
                name: "Gateway",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PaymentTransactionId",
                table: "Appointments",
                column: "PaymentTransactionId",
                unique: true,
                filter: "[PaymentTransactionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_PaymentTransactions_PaymentTransactionId",
                table: "Appointments",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
