using SV22T1020063.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.Models.Security;
using SV22T1020063.BusinessLayers;

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

            var userAccount = await UserAccountService.AuthorizeAsync(AccountTypes.Employee, username, hashedPassword);

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
                DisplayName = userAccount.DisplayName,
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
            ViewBag.Title = "Đổi mật khẩu";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Xác nhận mật khẩu không khớp");
                return View();
            }
            if (newPassword == oldPassword)
            {
                ModelState.AddModelError("", "Mật khẩu mới không được trùng với mật khẩu cũ");
                return View();
            }
            var userData = User.GetUserData();
            if (userData == null)
                return RedirectToAction("Login");

            // Kiểm tra mật khẩu cũ
            string hashedOldPassword = CryptHelper.HashMD5(oldPassword);
            var user = await UserAccountService.AuthorizeAsync(AccountTypes.Employee, userData.UserName!, hashedOldPassword);
            if (user == null)
            {
                ModelState.AddModelError("", "Mật khẩu cũ không chính xác");
                return View();
            }

            // Đổi mật khẩu
            string hashedNewPassword = CryptHelper.HashMD5(newPassword);
            bool result = await UserAccountService.ChangePasswordAsync(AccountTypes.Employee, userData.UserName!, hashedNewPassword);

            if (result)
            {
                ViewBag.Success = "Đổi mật khẩu thành công";
                return View();
            }
            else
            {
                ModelState.AddModelError("", "Đổi mật khẩu thất bại. Vui lòng thử lại sau.");
                return View();
            }
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
