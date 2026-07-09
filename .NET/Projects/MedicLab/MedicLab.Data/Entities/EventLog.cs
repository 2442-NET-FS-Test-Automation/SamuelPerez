namespace MedicLab.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

public class EventLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EventLogId {get; set;}
    public int AppointmentOrderId {get; set;}
    public AppointmentOrder AppointmentOrder {get; set;}
    public string? Message { get; set; }
    public string? MessageTemplate { get; set; }
    
    [MaxLength(128)]
    public string? Level { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }

}