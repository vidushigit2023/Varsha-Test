using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Models.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
