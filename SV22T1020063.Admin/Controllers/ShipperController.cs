using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;


namespace SV22T1020063.Admin.Controllers
{
    public class ShipperController : Controller
    {
        private const string SHIPPER_SEARCH = "ShipperSearchInput";
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
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
            
            var result = await PartnerDataService.ListShippersAsync(input);
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
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
