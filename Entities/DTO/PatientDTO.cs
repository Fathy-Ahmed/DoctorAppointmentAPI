using Entities.Models;
using System.ComponentModel.DataAnnotations;
namespace Entities.DTO;
public class PatientDTO
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string? MedicalHistory { get; set; }
    public string ApplicationUserId { get; set; }
    //////////////////////////////////////////////////////////////////
    
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
}