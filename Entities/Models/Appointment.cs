using System.ComponentModel.DataAnnotations.Schema;
using Utilities;

namespace Entities.Models;

public class Appointment
{
    public int Id { get; set; }


    public string? PatientName { get; set; }

    public string? Reason { get; set; }
    public TimeOnly Time { get; set; }

    public DateOnly Date { get; set; }

    public bool ForWho { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = SD.AppointmentIsNotVerified;


    //////////////////////////////////////////////////////////////////////////////////////////
    public int DoctorId { get; set; }
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; }
    //////////////////////////////////////////////////////////////////////////////////////////
    public int PatientId { get; set; }
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; }
    //////////////////////////////////////////////////////////////////////////////////////////

}
