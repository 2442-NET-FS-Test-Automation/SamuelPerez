using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace MedicLab.Data.Entities;

public class ClinicalStudy
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClinicalStudyId {get; set;}
    [Required, MaxLength(20)]
    public string LOINC {get; set;}
    [Required, MaxLength(300)] 
    public string StudyName {get; set;}
    [MaxLength(1000)]
    public string? Description {get; set;}

    public List<Availability?> Availability {get; set;} = new();
    public List<AppointmentDetail?> AppointmentDetails {get; set;} = new();
}