using DemoApplication.Models.Domain;
using DemoApplication.Models.ViewModels;
using DemoApplication.Repositories.Interface;
using DemoApplication.Repositories.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUser _userService;


        public AdminController(IUser userservice)
        {

            _userService = userservice;
        }
        [Authorize(Roles = "admin,user")]
        public IActionResult Display()
        {
            return View();
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UserList()
        {

            var users = await _userService.GetAllUsersAsync();
            UserDetails ulist = new UserDetails();
            ulist.Userlist = users;
            return View(ulist);
        }

        //Delete User from grid
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<Status> Delete(string Id)
        {
            var user = await _userService.GetUserDetailsAsync(Id);


            var status = await _userService.DeleteUserAsync(user);

            Status sboj = new Status();

            if (status == true)
            {
                sboj.StatusCode = 1;
                sboj.Message = "Data deleted successfully.";
            }
            else
            {
                sboj.StatusCode = 0;
                sboj.Message = "Data deleted Failed.";
            }

            return sboj;

        }
    }
}
