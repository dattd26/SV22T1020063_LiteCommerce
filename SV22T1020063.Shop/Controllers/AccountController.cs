using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Partner;
using SV22T1020063.Shop.Models;
using SV22T1020063.Shop.AppCodes;

namespace SV22T1020063.Shop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu";
                return View();
            }
            var userAccount = await UserAccountService.AuthorizeAsync(AccountTypes.Customer, username, CryptHelper.HashMD5(password));
            if (userAccount == null)
            {
                ViewBag.Error = "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin";
                return View();
            }

            var userData = new WebUserData
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                Roles = new List<string> { WebUserRoles.Customer }
            };

            var principal = userData.CreatePrincipal();
            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Customer data, string confirmPassword)
        {
            if (data.Password != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp");

            if (!await PartnerDataService.ValidatelCustomerEmailAsync(data.Email))
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi tài khoản khác");

            if (!ModelState.IsValid)
            {
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View(data);
            }

            data.CustomerID = 0;
            data.IsLocked = false;
            data.Password = CryptHelper.HashMD5(data.Password);
            int customerId = await PartnerDataService.AddCustomerAsync(data);
            if (customerId > 0)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Đăng ký không thành công. Vui lòng thử lại");
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View(data);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int customerId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var model = await PartnerDataService.GetCustomerAsync(customerId);
            if (model == null) return RedirectToAction("Logout");

            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Customer data)
        {
            if (!await PartnerDataService.ValidatelCustomerEmailAsync(data.Email, data.CustomerID))
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi tài khoản khác");

            if (!ModelState.IsValid)
            {
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View(data);
            }

            bool result = await PartnerDataService.UpdateCustomerAsync(data);
            if (result)
            {
                ViewBag.Message = "Cập nhật thông tin thành công";
                // Optionally update Identity claims if name/email changed
            }

            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp");
                return View();
            }

            string username = User.FindFirst("UserName")?.Value ?? "";
            var userAccount = await UserAccountService.AuthorizeAsync(AccountTypes.Customer, username, CryptHelper.HashMD5(oldPassword));
            if (userAccount == null)
            {
                ModelState.AddModelError("oldPassword", "Mật khẩu cũ không chính xác");
                return View();
            }

            bool result = await UserAccountService.ChangePasswordAsync(AccountTypes.Customer, username, CryptHelper.HashMD5(newPassword));
            if (result)
            {
                ViewBag.Message = "Đổi mật khẩu thành công";
            }
            else
            {
                ModelState.AddModelError("", "Đổi mật khẩu thất bại. Vui lòng thử lại");
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
