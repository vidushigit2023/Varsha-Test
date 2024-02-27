using Microsoft.AspNetCore.Identity;

namespace DemoApplication.Models.Domain
{
    public class Users:IdentityUser
    {
       
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
   
        public string? PinCode { get; set; }
        public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        public string? DocFile { get; set; }
    }

    public class UserDetails
    {
        public List<Users>? Userlist { get; set; }
    }
}
