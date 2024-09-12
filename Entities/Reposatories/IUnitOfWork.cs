namespace Entities.Reposatories;

public interface IUnitOfWork:IDisposable
{
    IDoctorRepository Doctor { get; }
    IPatientRepository Patient { get; }
    IAppointmentRepository Appointment { get; }

    Task<int> Save();
}
