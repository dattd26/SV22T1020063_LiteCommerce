using Microsoft.AspNetCore.Mvc;

namespace SV22T1020063.Admin.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
