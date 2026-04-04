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

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Trang chủ";
            
            // Lấy 12 sản phẩm tiêu biểu để hiển thị ở trang chủ
            var productResult = await CatalogDataService.ListProductsAsync(new ProductSearchInput
            {
                Page = 1,
                PageSize = 8, // Hiển thị 8 sản phẩm tiêu biểu
                SearchValue = "",
                CategoryID = 0,
                SupplierID = 0,
                MinPrice = 0,
                MaxPrice = 0
            });

            return View(productResult.DataItems);
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

            if (input.CategoryID > 0)
            {
                var category = await CatalogDataService.GetCategoryAsync(input.CategoryID);
                if (category != null)
                {
                    ViewBag.CategoryName = category.CategoryName;
                }
            }

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
