using Entities.Models;

namespace Entities.Reposatories;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    public List<Appointment> GetAppointmentsForDoctor(int DoctorId);
    public List<Appointment> GetAppointmentsForPatient(int PatientId);
}
