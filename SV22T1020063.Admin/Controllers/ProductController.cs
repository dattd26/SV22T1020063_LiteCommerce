using Microsoft.AspNetCore.Mvc;

namespace SV22T1020063.Admin.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            return View("Edit");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa mặt hàng";
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            return View();
        }
        public IActionResult ListAttributes(int pid)
        {
            return View();
        }
        public IActionResult CreateAttribute(int pid)
        {
            return View();
        }
        public IActionResult EditAttribute(int pid, int attributeId)
        {
            return View();
        } 
        public IActionResult DeleteAttribute(int pid, int attributeId)
        {
            return RedirectToAction("Edit", new { pid });
        }
        public IActionResult ListPhotos(int pid)
        {
            return View();
        }
        public IActionResult CreatePhoto(int pid)
        {
            ViewBag.Title = "Bổ sung ảnh";
            return View();
        }
        public IActionResult EditPhoto(int pid, int photoId)
        {

            ViewBag.Title = "Cập nhật ảnh";
            return View();
        }

        public IActionResult DeletePhoto(int pid, int photoId)
        {
            return RedirectToAction("Edit", new { pid });
        }
    }
}
