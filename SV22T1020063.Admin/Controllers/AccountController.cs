using SV22T1020063.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.Models.Security;

namespace SV22T1020063.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.UserName = username;

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }
            string hashedPassword = CryptHelper.HashMD5(password);

            // TODO: Lấy thông tin tài khoản dựa vào tên đăng nhập và mật khẩu
            //var userAccount = await SecurityDataService.AuthenticateEmployeeAsync(username, hashedPassword);

            // Giả lập tạm
            var userAccount = new UserAccount()
            {
                UserId = "1",
                UserName = username,
                DisplayName = "Dat",
                Email = username,
                Photo = "nophoto.png",
                RoleNames = $"{WebUserRoles.Administrator},{WebUserRoles.DataManager}" // admin,datamanager
            };
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            // Thông tin đăng nhập hpjw lệ:
            // Chuẩn bị thông tin mà sẽ ghi lên "giấy chứng nhận"
            var userData = new WebUserData()
            { 
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //Tạo ra giấy chứng nhận (ClaimPrincipal)
            var principal = userData.CreatePrincipal();
            
            // Trao chứng nhận cho phía client ,..,
            await HttpContext.SignInAsync(principal);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
