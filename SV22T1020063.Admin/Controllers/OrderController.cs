using Microsoft.AspNetCore.Mvc;

namespace SV22T1020063.Admin.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }
    }
}
