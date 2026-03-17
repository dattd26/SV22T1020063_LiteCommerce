using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;


namespace SV22T1020063.Admin.Controllers
{
    public class ShipperController : Controller
    {
        private const int PAGESIZE = 10;

        public async Task<IActionResult> Index(string searchValue = "", int page = 1)
        {
            var input = new PaginationSearchInput()
            {
                Page = page,
                PageSize = PAGESIZE,
                SearchValue = searchValue
            };
            ViewBag.SearchValue = searchValue;
            var result = await PartnerDataService.ListShippersAsync(input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            return View("Edit");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa người giao hàng";
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
