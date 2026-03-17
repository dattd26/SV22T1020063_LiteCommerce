using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private const int PAGESIZE = 10;
        public async Task<IActionResult> Index(int page = 1, string searchValue = "")
        {
            var input = new PaginationSearchInput()
            {
                Page = page,
                PageSize = PAGESIZE,
                SearchValue = searchValue
            };
            ViewBag.SearchValue = searchValue;
            var result = await PartnerDataService.ListSuppliersAsync(input);
            return View(result);
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
