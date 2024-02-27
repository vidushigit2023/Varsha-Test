using DemoApplication.Models.Domain;
using DemoApplication.Models.ViewModels;
using DemoApplication.Repositories.Interface;
using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace DemoApplication.Repositories.Services
{
    public class UserAuthenticationService:IUserAuthentication
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthenticationService(SignInManager<Users> signInManager, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<Status> ForgotPasswordAsync(PasswordModel model)
        {
            var status = new Status();
            try
            {
                //Get User by Email id
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //If User is exist then create token and send it through email, but now we have not configure email
                    var ResetPasswordToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    //Veryfy email token method
                    var tokenVerify = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", ResetPasswordToken);
                    //if Token is verify then update password
                    if (tokenVerify)
                    {
                        var isReset = await userManager.ResetPasswordAsync(user, ResetPasswordToken, model.Password);
                        if (isReset.Succeeded)
                        {
                            status.StatusCode = 1;
                            status.Message = "Password updated successfully";
                        }
                    }
                    else
                    {
                        status.StatusCode = 0;
                        status.Message = "Something wents wrong";
                    }

                }


            }
            catch (Exception)
            {
                status.StatusCode = 0;
                status.Message = "Something wents wrong";
            }
            return status;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.Username);       //Get User by username using identity nethod
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return status;
            }

            //Check password of the user
            if (!await userManager.CheckPasswordAsync(user, model.Password))    
            {
                status.StatusCode = 0;
                status.Message = "Invalid Password";
                return status;
            }

            //if all fields are correct the user will be login
            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true); //if all is correct the user will be login
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Logged in successfully";
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User is locked out";
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on logging in";
            }

            return status;
        }

        //Logout identity method
        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExist = await userManager.FindByNameAsync(model.Username);      //Check UserName us already exist or not
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
            var result = await userManager.CreateAsync(user, model.Password);       //Create user identity method
            if (!result.Succeeded)
            {
                var message = "";
                if (result.Errors.Count() > 0)
                {
                    foreach (var msg in result.Errors)
                    {
                        message = message + msg.Description;
                    }
                }
                else
                {
                    message = "Fail to create user";
                }

                status.StatusCode = 0;
                status.Message = message;
                return status;
            }

            //Role Manager method

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

        public async Task<Status> ForgotPasswordEmailAsync(PasswordModel model)
        {
            var status = new Status();
            try
            {
                //Get User by Email id
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    status.StatusCode = 1;
                    status.Message = "EmailId exist in our database.Check  your Inbox";
                }
                else
                {
                    status.StatusCode = 0;
                    status.Message = "Something wents wrong";
                }


            }
            catch (Exception)
            {
                status.StatusCode = 0;
                status.Message = "Something wents wrong";
            }
            return status;
        }
    }
}
