using Microsoft.EntityFrameworkCore;
using MedicLab.Data.Entities;

namespace MedicLab.Data;

public class MedicLabDbContext : DbContext
{
    public MedicLabDbContext(DbContextOptions<MedicLabDbContext> options) : base(options){}

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Availability> Availability => Set<Availability>();
    public DbSet<ClinicalStudy> ClinicalStudies => Set<ClinicalStudy>();
    public DbSet<AppointmentOrder> AppointmentOrders => Set<AppointmentOrder>();
    public DbSet<AppointmentDetail> AppointmentDetails => Set<AppointmentDetail>();
    public DbSet<FulfillmentEvent> FulfillmentEvents => Set<FulfillmentEvent>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<EventLog> EventLogs => Set<EventLog>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Patient>(e =>
        {
           e.HasIndex(p => p.CURP, "UQ_Patient_Curp").IsUnique();
           e.HasIndex(p => p.Email, "UQ_Patient_Email").IsUnique();
           e.ToTable(p => p.HasCheckConstraint("CK_Patient_Birthdate", "[BirthDate] <= CONVERT(date, GETDATE())"));
        });

        b.Entity<ClinicalStudy>(e =>
        {
            e.HasIndex(cs => cs.LOINC, "UQ_ClinicalStudies").IsUnique();
        });

        b.Entity<Availability>(e =>
        {
            e.ToTable(a => a.HasCheckConstraint("CK_ClinicalStudies_AvailableDays", "[Day] >= CONVERT(date, GETDATE())"));
            e.Property(a => a.RowVersion).IsRowVersion();
            e.HasIndex(a => new { a.ClinicalStudyId, a.Day}).IsUnique();
        });

        b.Entity<Patient>().HasData(
            new Patient { PatientId = 1, CURP = "PEAS030328HJCRLMA4", Email = "Samuelpalfaro@gmail.com", FirstName = "Samuel", LastName = "Pérez", BirthDate = new DateTime(2003, 03, 28) },
            new Patient { PatientId = 2, CURP = "SABC5560626MDFLRN0", Email = "Concepcion@mail.com", FirstName = "Concepción", LastName = "Salgado", BirthDate = new DateTime(1956, 06, 26) }
        );

        b.Entity<Doctor>().HasData(
            new Doctor { DoctorId = 1, FirstName = "Mario", LastName = "Ruiz", MedicalLicense = "AR76097"},
            new Doctor { DoctorId = 2, FirstName = "Luisa", LastName = "Romero", MedicalLicense = "TR42850"}
        );

        b.Entity<ClinicalStudy>().HasData(
            new ClinicalStudy { ClinicalStudyId = 1, LOINC = "2345-7", StudyName = "Glucose [Mass/volume] in Blood", Description = "It measures the amount of glucose mass per unit volume present in a blood sample."},
            new ClinicalStudy { ClinicalStudyId = 2, LOINC = "2546-9", StudyName = "Glucose [Mass/volume] in 24 hour Urine", Description = "It specifically identifies the glucose concentration, but changes the system (urine) and timing (24-hour collection)."},
            new ClinicalStudy { ClinicalStudyId = 3, LOINC = "94500-6", StudyName = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection", Description = "Indicates the presence of the SARS-CoV-2 virus (qualitative) using a nucleic acid amplification test (such as PCR) in a respiratory specimen."}
        );

        b.Entity<Availability>().HasData(
            new Availability { AvailabilityId = 1, ClinicalStudyId = 2, DurationMinutes = 10, Slots = 20, Day = new DateTime(2026, 07, 09)},
            new Availability { AvailabilityId = 2, ClinicalStudyId = 3, DurationMinutes = 5, Slots = 3, Day = new DateTime(2026, 07, 09)},
            new Availability { AvailabilityId = 3, ClinicalStudyId = 1, DurationMinutes = 20, Slots = 15, Day = new DateTime(2026, 07, 10)},
            new Availability { AvailabilityId = 4, ClinicalStudyId = 3, DurationMinutes = 5, Slots = 4, Day = new DateTime(2026, 07, 10)}
        );

    }
}