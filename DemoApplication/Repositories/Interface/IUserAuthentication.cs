using DemoApplication.Models.ViewModels;


namespace DemoApplication.Repositories.Interface
{
    public interface IUserAuthentication
    {
        Task<Status> LoginAsync(LoginModel model);
        Task<Status> RegistrationAsync(RegistrationModel model);
        Task LogoutAsync();
        Task<Status> ForgotPasswordAsync(PasswordModel model);

        Task<Status> ForgotPasswordEmailAsync(PasswordModel model);
    }
}
