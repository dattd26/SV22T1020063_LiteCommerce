using Microsoft.AspNetCore.Mvc;

namespace SV22T1020063.Admin.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            return View("Edit");
        }
        public IActionResult Edit()
        {
            ViewBag.Title = "Chỉnh sửa khách hàng";
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}
