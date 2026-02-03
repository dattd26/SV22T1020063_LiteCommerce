using Microsoft.AspNetCore.Mvc;

namespace SV22T1020063.Admin.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}
