using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private const string SUPPLIER_SEARCH = "SupplierSearchInput";
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = string.Empty
                };
            }
            return View(input);
        }
        public async Task<IActionResult> Search(PaginationSearchInput input) {
            
            var result = await PartnerDataService.ListSuppliersAsync(input);
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
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
