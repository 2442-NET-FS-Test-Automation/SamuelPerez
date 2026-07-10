using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClinicalStudies",
                columns: table => new
                {
                    ClinicalStudyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LOINC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StudyName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalStudies", x => x.ClinicalStudyId);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MedicalLicense = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorId);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CURP = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.CheckConstraint("CK_Patient_Birthdate", "[BirthDate] <= CONVERT(date, GETDATE())");
                });

            migrationBuilder.CreateTable(
                name: "Availability",
                columns: table => new
                {
                    AvailabilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClinicalStudyId = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Slots = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<DateTime>(type: "date", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availability", x => x.AvailabilityId);
                    table.CheckConstraint("CK_ClinicalStudies_AvailableDays", "[Day] >= CONVERT(date, GETDATE())");
                    table.ForeignKey(
                        name: "FK_Availability_ClinicalStudies_ClinicalStudyId",
                        column: x => x.ClinicalStudyId,
                        principalTable: "ClinicalStudies",
                        principalColumn: "ClinicalStudyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentOrders",
                columns: table => new
                {
                    AppointmentOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentOrders", x => x.AppointmentOrderId);
                    table.ForeignKey(
                        name: "FK_AppointmentOrders_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentOrders_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentDetails",
                columns: table => new
                {
                    AppointmentDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentOrderId = table.Column<int>(type: "int", nullable: false),
                    ClinicalStudyId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDetails", x => x.AppointmentDetailId);
                    table.ForeignKey(
                        name: "FK_AppointmentDetails_AppointmentOrders_AppointmentOrderId",
                        column: x => x.AppointmentOrderId,
                        principalTable: "AppointmentOrders",
                        principalColumn: "AppointmentOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentDetails_ClinicalStudies_ClinicalStudyId",
                        column: x => x.ClinicalStudyId,
                        principalTable: "ClinicalStudies",
                        principalColumn: "ClinicalStudyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventLogs",
                columns: table => new
                {
                    EventLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentOrderId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TimeStamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.EventLogId);
                    table.ForeignKey(
                        name: "FK_EventLogs_AppointmentOrders_AppointmentOrderId",
                        column: x => x.AppointmentOrderId,
                        principalTable: "AppointmentOrders",
                        principalColumn: "AppointmentOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FulfillmentEvents",
                columns: table => new
                {
                    FulfillmentEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentOrderId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FulfilledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillmentEvents", x => x.FulfillmentEventId);
                    table.ForeignKey(
                        name: "FK_FulfillmentEvents_AppointmentOrders_AppointmentOrderId",
                        column: x => x.AppointmentOrderId,
                        principalTable: "AppointmentOrders",
                        principalColumn: "AppointmentOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ClinicalStudies",
                columns: new[] { "ClinicalStudyId", "Description", "LOINC", "StudyName" },
                values: new object[,]
                {
                    { 1, "It measures the amount of glucose mass per unit volume present in a blood sample.", "2345-7", "Glucose [Mass/volume] in Blood" },
                    { 2, "It specifically identifies the glucose concentration, but changes the system (urine) and timing (24-hour collection).", "2546-9", "Glucose [Mass/volume] in 24 hour Urine" },
                    { 3, "Indicates the presence of the SARS-CoV-2 virus (qualitative) using a nucleic acid amplification test (such as PCR) in a respiratory specimen.", "94500-6", "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "DoctorId", "FirstName", "LastName", "MedicalLicense" },
                values: new object[,]
                {
                    { 1, "Mario", "Ruiz", "AR76097" },
                    { 2, "Luisa", "Romero", "TR42850" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "PatientId", "BirthDate", "CURP", "Email", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, new DateTime(2003, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "PEAS030328HJCRLMA4", "Samuelpalfaro@gmail.com", "Samuel", "Pérez" },
                    { 2, new DateTime(1956, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "SABC5560626MDFLRN09", "Concepcion@mail.com", "Concepción", "Salgado" }
                });

            migrationBuilder.InsertData(
                table: "Availability",
                columns: new[] { "AvailabilityId", "ClinicalStudyId", "Day", "DurationMinutes", "Slots" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 20 },
                    { 2, 3, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3 },
                    { 3, 1, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, 15 },
                    { 4, 3, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetails_AppointmentOrderId",
                table: "AppointmentDetails",
                column: "AppointmentOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetails_ClinicalStudyId",
                table: "AppointmentDetails",
                column: "ClinicalStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentOrders_DoctorId",
                table: "AppointmentOrders",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentOrders_PatientId",
                table: "AppointmentOrders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_ClinicalStudyId",
                table: "Availability",
                column: "ClinicalStudyId");

            migrationBuilder.CreateIndex(
                name: "UQ_ClinicalStudies",
                table: "ClinicalStudies",
                column: "LOINC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_AppointmentOrderId",
                table: "EventLogs",
                column: "AppointmentOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentEvents_AppointmentOrderId",
                table: "FulfillmentEvents",
                column: "AppointmentOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Patient_Curp",
                table: "Patients",
                column: "CURP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Patient_Email",
                table: "Patients",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentDetails");

            migrationBuilder.DropTable(
                name: "Availability");

            migrationBuilder.DropTable(
                name: "EventLogs");

            migrationBuilder.DropTable(
                name: "FulfillmentEvents");

            migrationBuilder.DropTable(
                name: "ClinicalStudies");

            migrationBuilder.DropTable(
                name: "AppointmentOrders");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
