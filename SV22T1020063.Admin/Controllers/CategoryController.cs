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
<<<<<<< HEAD
        public IActionResult Edit(int id)
=======
        public IActionResult Edit()
>>>>>>> 47b8d31b5cba9f97d73b91bc1c666abcd974b432
        {
            ViewBag.Title = "Chỉnh sửa loại hàng";
            return View();
        }
<<<<<<< HEAD
        public IActionResult Delete(int id)
=======
        public IActionResult Delete()
>>>>>>> 47b8d31b5cba9f97d73b91bc1c666abcd974b432
        {
            return View();
        }
    }
}
