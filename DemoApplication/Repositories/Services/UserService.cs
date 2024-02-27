using DemoApplication.Models.Domain;
using DemoApplication.Models.ViewModels;
using DemoApplication.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Repositories.Services
{
    public class UserService:IUser
    {

        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        //Get User details by id
        public async Task<Users> GetUserDetailsAsync(string UserId)      
        {
            try
            {

                return await userManager.FindByIdAsync(UserId);

            }
            catch (Exception)
            {

                throw;
            }
        }
        //Update user 
        public async Task<bool> UpdateUserAsync(Users singleuser)
        {
            try
            {
                var a = await userManager.UpdateAsync(singleuser);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //Get All user list for admin
        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await userManager.Users.ToListAsync();
          
        }
        public async Task<bool> DeleteUserAsync(Users user)
        {
            try
            {
                var a = await userManager.DeleteAsync(user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //Add User 
        public async Task<Status> AddUserAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExist = await userManager.FindByNameAsync(model.Username);
            if (userExist != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exist";
                return status;
            }
            Users user = new Users
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            //Role Manager

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));


            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;
        }
    }
}
