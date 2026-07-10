using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class Availability_CompositeIx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Availability_ClinicalStudyId",
                table: "Availability");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_ClinicalStudyId_Day",
                table: "Availability",
                columns: new[] { "ClinicalStudyId", "Day" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Availability_ClinicalStudyId_Day",
                table: "Availability");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_ClinicalStudyId",
                table: "Availability",
                column: "ClinicalStudyId");
        }
    }
}
