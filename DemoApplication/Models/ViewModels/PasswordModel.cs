using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Models.ViewModels
{
    public class PasswordModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
