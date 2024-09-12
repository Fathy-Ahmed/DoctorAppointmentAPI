using DataAccess.Data;
using Entities.Reposatories;

namespace DataAccess.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public  IDoctorRepository Doctor { get; private set; }
    public IPatientRepository Patient { get; private set; }
    public IAppointmentRepository Appointment { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        this._context = context;
        Doctor = new DoctorRepository(_context);
        Patient = new PatientRepository(_context);
        Appointment = new AppointmentRepository(_context);
    }

    public async Task<int> Save()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
