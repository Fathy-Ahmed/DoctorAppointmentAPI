namespace Entities.DTO;

public class AppointmentRescheduleDTO
{
    public int AppointmentId { get; set; }
    public int DocotrId { get; set; }
    public TimeOnly Time { get; set; }
	public DateOnly Date { get; set; }
}
