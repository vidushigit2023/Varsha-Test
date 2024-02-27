using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Models.ViewModels
{
    public class RegistrationModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Minimum length 6 and must contain  1 Uppercase,1 lowercase, 1 special character and 1 digit")]
        public string Password { get; set; }

        [Compare("Password")]
        public string PasswordConfirm { get; set; }
        public string? Role { get; set; }
    }
}
