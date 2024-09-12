using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTO;

public class DoctorDTO
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Gender { get; set; }
	public string Img { get; set; }
	public string Specialties { get; set; }
	public string Qualification { get; set; }
	public int ExperienceYears { get; set; }
	public string ClinicLocation { get; set; }
	public string FromDay { get; set; }
	public string ToDay { get; set; }
	public TimeSpan StartTime { get; set; }
	public TimeSpan EndTime { get; set; }

	public int NumberOfPatientInDay { get; set; }
	public string? ApplicationUserId { get; set; }

	//-----------------------------------------------------------------------------------
	[DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }

	//-----------------------------------------------------------------------------------

    public IFormFile? file { get; set; }


}
