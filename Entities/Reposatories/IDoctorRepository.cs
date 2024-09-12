using Entities.Models;

namespace Entities.Reposatories;

public interface IDoctorRepository:IGenericRepository<Doctor>
{
    public Task<Doctor> GetByUserId(string Id);
    public Task<int> CountAcceptedDoctoes();
    public Task<int> CountRequestedDoctoes();
}
