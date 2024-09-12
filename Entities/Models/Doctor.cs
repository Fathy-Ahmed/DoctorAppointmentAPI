using System.ComponentModel.DataAnnotations.Schema;
using Utilities;

namespace Entities.Models;

public class Doctor
{
    public int Id { get; set; }
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
    public string Status { get; set; } = SD.DoctorIsPending;


    //////////////////////////////////////////////////////////////////////////////////
    public string ApplicationUserId { get;set; }
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get;set; }



    //////////////////////////////////////////////////////////////////////////////////

    public ICollection<Appointment> Appointments { get; set; }

    //////////////////////////////////////////////////////////////////////////////////
    ///
}
