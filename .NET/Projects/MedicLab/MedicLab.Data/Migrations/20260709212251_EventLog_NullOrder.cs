using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventLog_NullOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventLogs_AppointmentOrders_AppointmentOrderId",
                table: "EventLogs");

            migrationBuilder.DropIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentOrderId",
                table: "EventLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs",
                column: "AppointmentOrderId",
                unique: true,
                filter: "[AppointmentOrderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EventLogs_AppointmentOrders_AppointmentOrderId",
                table: "EventLogs",
                column: "AppointmentOrderId",
                principalTable: "AppointmentOrders",
                principalColumn: "AppointmentOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventLogs_AppointmentOrders_AppointmentOrderId",
                table: "EventLogs");

            migrationBuilder.DropIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentOrderId",
                table: "EventLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs",
                column: "AppointmentOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventLogs_AppointmentOrders_AppointmentOrderId",
                table: "EventLogs",
                column: "AppointmentOrderId",
                principalTable: "AppointmentOrders",
                principalColumn: "AppointmentOrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
