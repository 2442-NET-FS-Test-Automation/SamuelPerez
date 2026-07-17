using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace MedicLab.Data.Entities;

public class Doctor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DoctorId {get; set;}
    [Required, MaxLength(50)]
    public string FirstName {get; set;}
    [Required, MaxLength(50)]
    public string LastName {get; set;}
    [Required, MaxLength(10)]
    public string MedicalLicense {get; set;}

    public List<AppointmentOrder> AppointmentOrders {get; set;} = new();
}