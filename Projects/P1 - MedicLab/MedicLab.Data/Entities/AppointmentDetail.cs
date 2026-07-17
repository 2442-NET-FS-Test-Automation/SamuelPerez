using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace MedicLab.Data.Entities;

public class AppointmentDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AppointmentDetailId {get; set;}
    public int AppointmentOrderId {get; set;}
    public AppointmentOrder AppointmentOrder {get; set;}
    public int  ClinicalStudyId {get; set;}
    public ClinicalStudy ClinicalStudy {get; set;}
    public int Quantity {get; set;}
    [Required, Column(TypeName = "date")]
    public DateTime Date {get; set;}
}