namespace Entities.DTO;

public class PatientAppointmentsDTO
{
    public int AppointmentId { get; set; }
    public string? Reason { get; set; } = "No Reason Specified";
    public TimeOnly Time { get; set; }
    public DateOnly Date { get; set; }
    public string Status { get; set; }
    //---------------------------------------------
    public int DoctorId { get; set; }
    public string DoctorName { get; set; }
    public string DoctorImg { get; set; }
    public string PatientEmail { get; set; }
    public string DoctorPhoneNumber { get; set; }
    public string? DoctorClinicName { get; set; }
    public string ClinicLocation { get; set; }
    //---------------------------------------------

}
