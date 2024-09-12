using System.ComponentModel.DataAnnotations;
namespace Entities.DTO
{
    public class RegisterDTO
    {
        [Length(1,20)]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmedPassword { get; set; }

        public bool IsDoctor { get; set; }
    }
}
