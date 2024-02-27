using DemoApplication.Models.ViewModels;
using DemoApplication.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Controllers
{
    public class RegistrationController : Controller
    {

        private readonly IUserAuthentication _authService;
        public RegistrationController(IUserAuthentication authService)
        {
            this._authService = authService;
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
                return View("~/UserLogin/UserLogin");
            }

            return RedirectToAction(nameof(Registration));
        }
    }
}
