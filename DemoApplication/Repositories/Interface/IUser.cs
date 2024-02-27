using DemoApplication.Models.Domain;
using DemoApplication.Models.ViewModels;


namespace DemoApplication.Repositories.Interface
{
    public interface IUser
    {
        Task<Users> GetUserDetailsAsync(string Username);
        Task<bool> UpdateUserAsync(Users user);
        Task<List<Users>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(Users singleuser);
        Task<Status> AddUserAsync(RegistrationModel model);
    }
}
