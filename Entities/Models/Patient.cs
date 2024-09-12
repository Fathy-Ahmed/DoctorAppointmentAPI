using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;
public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string? MedicalHistory { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get; set; }




    public ICollection<Appointment> Appointments { get; set; }

}
