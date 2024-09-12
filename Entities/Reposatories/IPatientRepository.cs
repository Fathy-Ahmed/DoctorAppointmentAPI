using Entities.Models;

namespace Entities.Reposatories;

public interface IPatientRepository : IGenericRepository<Patient>
{
    public Task<Patient> GetByUserId(string Id);
}
