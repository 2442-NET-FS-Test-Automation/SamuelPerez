using MedicLab.Data;
using MedicLab.Data.Entities;
using Microsoft.EntityFrameworkCore;

public interface ISeeder
{
    public IReadOnlyList<int> SeedAppointmentOrders(int ordersAmount, bool urgents);
}

public class Seeder : ISeeder
{
    private static readonly string[] LOINCs = {"2345-7", "2546-9", "94500-6"};

    private readonly IDbContextFactory<MedicLabDbContext> _factory;

    public Seeder(IDbContextFactory<MedicLabDbContext> factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<int> SeedAppointmentOrders(int ordersAmount, bool urgents)
    {
        using MedicLabDbContext? db = _factory.CreateDbContext();
        Dictionary<string, int>? sIds = db.ClinicalStudies.ToDictionary(cs => cs.LOINC, cs => cs.ClinicalStudyId);

        List<int> ids = new List<int>(ordersAmount);

        for (int i = 0; i < ordersAmount; i++)
        {
            AppointmentOrder appointmentOrder = new AppointmentOrder
            {
                PatientId = Random.Shared.Next(1, 3),
                DoctorId = Random.Shared.Next(1, 3),
                Priority = urgents ? Priority.Expedited : Priority.Normal,
                Status = Status.Pending,
                Details  = { 
                    new AppointmentDetail { ClinicalStudyId = sIds[LOINCs[Random.Shared.Next(0, LOINCs.Length)]], Quantity = Random.Shared.Next(1, 5), Date = new DateTime(2026,07,09)},
                    new AppointmentDetail { ClinicalStudyId = sIds[LOINCs[Random.Shared.Next(0, LOINCs.Length)]], Quantity = Random.Shared.Next(1, 5), Date = new DateTime(2026,07,10)},
                }
            };
            db.AppointmentOrders.Add(appointmentOrder);
            db.SaveChanges();
            ids.Add(appointmentOrder.AppointmentOrderId);
        }
        return ids;
    }
}