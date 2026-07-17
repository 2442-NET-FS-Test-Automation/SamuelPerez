using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace MedicLab.Data.Entities;

public class AppointmentOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AppointmentOrderId {get; set;}
    [Required]
    public int PatientId {get; set;}
    public Patient Patient {get; set;} = default!;
    public int DoctorId {get; set;}
    public Doctor Doctor {get; set;} = default!;
    public Priority Priority {get; set;}
    public Status Status {get; set;}
    public DateTime CreatedUtc {get; set;} = DateTime.UtcNow;
    public DateTime? CompletedUtc {get; set;}

    public List<AppointmentDetail> Details {get; set;} = new();
    public FulfillmentEvent? FulfillmentEvent {get; set;}
    public List<EventLog?> EventLog {get; set;}
}