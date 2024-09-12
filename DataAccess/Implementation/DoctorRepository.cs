using DataAccess.Data;
using Entities.Models;
using Entities.Reposatories;
using Utilities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementation;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    private readonly AppDbContext _context;

    public DoctorRepository(AppDbContext context) : base(context)
    {
        this._context = context;
    }

	public async Task<int> CountAcceptedDoctoes()
	{
		return await _context.Doctors.Where(e=>e.Status==SD.DoctorIsAprovied).CountAsync();
	}

	public async Task<int> CountRequestedDoctoes()
	{
		return await _context.Doctors.Where(e => e.Status == SD.DoctorIsPending).CountAsync();
	}

	public async Task<Doctor> GetByUserId(string Id)
    {
        return  await _context.Doctors.FirstOrDefaultAsync(x => x.ApplicationUserId == Id);
    }



}
