using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventLogs_OrderId_Restriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs",
                column: "AppointmentOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs",
                column: "AppointmentOrderId",
                unique: true,
                filter: "[AppointmentOrderId] IS NOT NULL");
        }
    }
}
