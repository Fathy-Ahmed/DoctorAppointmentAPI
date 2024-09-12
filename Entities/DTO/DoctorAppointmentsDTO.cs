namespace Entities.DTO;

public class DoctorAppointmentsDTO
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; }
    public string PatientEmail { get; set; }
    public string PatientPhoneNumber { get; set; }
    public string? Reason { get; set; }
    public TimeOnly Time { get; set; }
    public DateOnly Date { get; set; }
    public string? Message { get; set; }
	public string Status { get; set; }
}
