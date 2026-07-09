using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicLab.Data.Entities;

public class FulfillmentEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FulfillmentEventId {get; set;}
    public int AppointmentOrderId {get; set;}
    public AppointmentOrder AppointmentOrder {get; set;} = default!;
    public string Type {get; set;} = default!;
    public DateTime FulfilledAtUtc {get; set;} = DateTime.UtcNow;


}