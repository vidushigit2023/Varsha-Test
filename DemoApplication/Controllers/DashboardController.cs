using DemoApplication.Models.Domain;
using DemoApplication.Models.ViewModels;
using DemoApplication.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace DemoApplication.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUser _userService;

        public DashboardController(IUser userService)
        {
            _userService = userService;
        }

        public IActionResult Display()
        {
            return View();
        }
        //view user profile
       [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Profile(string userId = null)
        {
            if (userId == null)
            {
                userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }

            Users userinfo = await _userService.GetUserDetailsAsync(userId);
            UserModel updateUserModel = new UserModel();
            updateUserModel.UserName = userinfo.UserName;
            updateUserModel.PhoneNumber = userinfo.PhoneNumber;
            updateUserModel.LastName = userinfo.LastName;
            updateUserModel.FirstName = userinfo.FirstName;
            updateUserModel.ProfilePicture = userinfo.ProfilePicture;
            updateUserModel.DocFile = userinfo.DocFile;
            updateUserModel.Address = userinfo.Address;
            
            TempData["UserId"] = userId;
            return View(updateUserModel);

        }
        [Authorize(Roles = "admin,user")]
        [HttpPost]
        public async Task<IActionResult> Profile(UserModel model)
        {
            string userId;
            if (TempData["UserId"] != null)
            {
                userId = TempData["UserId"].ToString();
            }
            else
            {
                userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }

            Users userinfo = await _userService.GetUserDetailsAsync(userId);
            //File location of upload
            string uploadPath = "wwwroot//ProfileImage";
            string docuploadPath = "wwwroot//DocFiles";

            var files = HttpContext.Request.Form.Files;
            if (files.Count != 0)
            {
                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        if (file.Name == "ProfilePicture")
                        {
                            //Rename files
                            var fileName = files[0].FileName.Split('.')[0].ToString().Replace("/", "-").Replace(" ", "-") + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + Path.GetExtension(file.FileName);
                            var uploadPathWithfileName = Path.Combine(uploadPath, fileName);
                            var uploadAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), uploadPathWithfileName);
                            using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);

                                userinfo.ProfilePicture = fileName;


                            }
                        }
                        else if (file.Name == "DocFile")
                        {
                            //Rename files
                            var fileName = files[0].FileName.Split('.')[0].ToString().Replace("/", "-").Replace(" ", "-") + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + Path.GetExtension(file.FileName);
                            var uploadPathWithfileName = Path.Combine(docuploadPath, fileName);
                            var uploadAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), uploadPathWithfileName);
                            using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);

                                userinfo.DocFile = fileName;


                            }
                        }
                    }
                }
            }


            userinfo.PhoneNumber = model.PhoneNumber;
            userinfo.LastName = model.LastName;
            userinfo.FirstName = model.FirstName;
            userinfo.Address = model.Address;
            var result = await _userService.UpdateUserAsync(userinfo);
            if (result)
            {
               // string userId=model.userI
                // ViewData["msg"] = "Profile Updated Successfully."; 
                return RedirectToAction("Profile", "Dashboard", userId);
               // return View(model);
            }
            else
            {
               // ViewData["msg"] = "Profile Update Fail.";
                return View(model);
            }
        }

        //If user try to access unauthorize page from url or something else, then this redirect to access denide page
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }


        
        [HttpPost]
        public async Task<Status> DeletePic()
        {
            string userId = null;
            if (TempData["UserId"] != null)
            {
                userId = TempData["UserId"].ToString();
            }
            else
            {
                userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            var userinfo = await _userService.GetUserDetailsAsync(userId);



            userinfo.ProfilePicture = null;
            var result = await _userService.UpdateUserAsync(userinfo);

            
            Status sboj = new Status();
            if (result)
            {
                sboj.StatusCode = 1;
                sboj.Message = "Profile Pic deleted successfully.";
            }
            else
            {
                sboj.StatusCode = 0;
                sboj.Message = "Profile Pic deleted Failed.";
            }
          
            return sboj;

        }


        [HttpPost]
        public async Task<Status> DeleteDoc()
        {
            string userId = null;

            if (TempData["UserId"] != null)
            {
                userId = TempData["UserId"].ToString();
            }
            else
            {
                userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            var userinfo = await _userService.GetUserDetailsAsync(userId);

            userinfo.DocFile =null;
            var result = await _userService.UpdateUserAsync(userinfo);

            Status sboj = new Status();
            if (result)
            {
                sboj.StatusCode = 1;
                sboj.Message = "Doc deleted successfully.";
            }
            else
            {
                sboj.StatusCode = 0;
                sboj.Message = "Doc deleted Failed.";
            }
            return sboj;

        }
    }
}
