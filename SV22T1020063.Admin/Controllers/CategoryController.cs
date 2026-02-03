using Microsoft.AspNetCore.Mvc;

namespace SV22T1020063.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            return View("Edit");
        }
        public IActionResult Edit()
        {
            ViewBag.Title = "Chỉnh sửa loại hàng";
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}
