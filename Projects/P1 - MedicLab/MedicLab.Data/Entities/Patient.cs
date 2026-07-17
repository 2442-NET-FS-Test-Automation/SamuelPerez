using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace MedicLab.Data.Entities;

public class Patient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PatientId {get; set;}
    [Required]
    [StringLength(18, MinimumLength = 18, ErrorMessage = "The field must be exactly 18 characters long.")]
    public string CURP {get; set;}
    [Required, EmailAddress, MaxLength(150)]
    public string Email {get; set;}
    [Required, MaxLength(50)]
    public string FirstName {get; set;}
    [Required, MaxLength(50)]
    public string LastName {get; set;}
    [Required, Column(TypeName = "date")]
    public DateTime BirthDate {get; set;}

    public List<AppointmentOrder> AppointmentOrders {get; set;} = new(); 

}