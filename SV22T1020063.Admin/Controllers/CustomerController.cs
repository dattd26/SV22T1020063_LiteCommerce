using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Admin.Controllers
{
    public class CustomerController : Controller
    {
        private const int PAGESIZE = 20;

        public async Task<IActionResult> Index(int page = 1, string searchValue = "")
        {
            var input = new PaginationSearchInput()
            {
                Page = page,
                PageSize = PAGESIZE,
                SearchValue = searchValue
            };
            ViewBag.SearchValue = searchValue;
            var result = await PartnerDataService.ListCustomerAsync(input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            return View("Edit");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa khách hàng";
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }

        public IActionResult ChangePassword(int id)
        {
            return View();
        }
    }
}
