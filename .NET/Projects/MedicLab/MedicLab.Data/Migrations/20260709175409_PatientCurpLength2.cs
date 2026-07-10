using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class PatientCurpLength2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 2,
                column: "CURP",
                value: "SABC5560626MDFLRN0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 2,
                column: "CURP",
                value: "SABC5560626MDFLRN09");
        }
    }
}
