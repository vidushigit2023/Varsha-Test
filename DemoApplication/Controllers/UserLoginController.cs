using DemoApplication.Models.ViewModels;
using DemoApplication.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly IUserAuthentication _authService;
        public UserLoginController(IUserAuthentication authService)
        {
            this._authService = authService;
        }
        public IActionResult UserLogin()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _authService.LoginAsync(model);
            if (result.StatusCode == 1)
            {
                return RedirectToAction("Display", "Dashboard");
            }
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(UserLogin));
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await this._authService.LogoutAsync();
            return RedirectToAction(nameof(UserLogin));
        }


        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            await Task.Delay(100);
            return View();
        }

        //Post Method forgot password
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(PasswordModel model)
        {
            var result = await this._authService.ForgotPasswordEmailAsync(model);
            if (result.StatusCode == 1)
            {
                TempData["msg"] = result.Message;
                return View("UserLogin");
            }
            return View();

        }

        public IActionResult Registration()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (!ModelState.IsValid) { return View(model); }
            model.Role = "user";
            var result = await this._authService.RegistrationAsync(model);
            TempData["msg"] = result.Message;
            if (result.StatusCode == 1)
            {
                return View("UserLogin");
            }

            return RedirectToAction(nameof(Registration));
        }



        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin()
        {
            RegistrationModel model = new RegistrationModel
            {
                Username = "admin",
                Email = "admin@gmail.com",
                FirstName = "Vedansh",
                LastName = "Kanase",
                Password = "Admin@123"
            };
            model.Role = "admin";
            var result = await _authService.RegistrationAsync(model);
            return Ok(result);
        }
    }
}
