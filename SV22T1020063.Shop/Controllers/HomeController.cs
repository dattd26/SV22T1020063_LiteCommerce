using Microsoft.AspNetCore.Mvc;
using SV22T1020063.Shop.Models;
using System.Diagnostics;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Catalog;

namespace SV22T1020063.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchValue = "", int categoryID = 0)
        {
            ViewBag.Title = "Trang chủ";
            ViewBag.SearchValue = searchValue;
            ViewBag.CategoryID = categoryID;
            
            var categories = await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput
            {
                Page = 1,
                PageSize = 100, // Get all categories for sidebar
                SearchValue = ""
            });
            ViewBag.Categories = categories.DataItems;
            return View();
        }

        public async Task<IActionResult> Search(ProductSearchInput input)
        {
            int pageSize = 12; // Standard grid size
            var result = await CatalogDataService.ListProductsAsync(new ProductSearchInput
            {
                Page = input.Page,
                PageSize = pageSize,
                SearchValue = input.SearchValue ?? "",
                CategoryID = input.CategoryID,
                SupplierID = 0,
                MinPrice = input.MinPrice,
                MaxPrice = input.MaxPrice
            });
            return PartialView(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
