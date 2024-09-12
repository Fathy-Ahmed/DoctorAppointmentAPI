using Utilities;

namespace Entities.DTO;

public class AppointmentDTO
{
    public string? PatientName { get; set; }

    public string? Reason { get; set; }
    public TimeOnly Time { get; set; }

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public bool ForWho { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = SD.AppointmentIsNotVerified;
    ////////////////////////////////////////////////////////
    public int DocotrId { get; set; }
    public string DocotrName { get; set; }
    public string DocotrImg { get; set; }
    public string DocotrSpecialties { get; set; }
    public string ClinicLocation { get; set; }
    ////////////////////////////////////////////////////////
    public int PatientId { get; set; }
}
