using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicLab.Data.Entities;

#pragma warning disable CS8618

public class Availability
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AvailabilityId {get; set;}
    public int ClinicalStudyId {get; set;}
    public ClinicalStudy ClinicalStudy {get; set;} = default!;
    [Required, Range(0, 180)]
    public int DurationMinutes {get; set;}
    [Required, Range(0, 100)]
    public int Slots {get; set;}
    [Required, Column(TypeName = "date")]
    public DateTime Day {get; set;}
    public byte[] RowVersion {get; set;}
}