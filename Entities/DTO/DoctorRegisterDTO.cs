using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTO;
public class DoctorRegisterDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public string? Img { get; set; }
    public string Specialties { get; set; }
    public string Qualification { get; set; }
    public int ExperienceYears { get; set; }
    public string ClinicLocation { get; set; }
    public string FromDay { get; set; }
    public string ToDay { get; set; }
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    public int NumberOfPatientInDay { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile? file { get; set; }    

}
