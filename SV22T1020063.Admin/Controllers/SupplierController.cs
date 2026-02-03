using Microsoft.AspNetCore.Mvc;

namespace SV22T1020063.Admin.Controllers
{
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            return View("Edit");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa nhà cung cấp";
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
