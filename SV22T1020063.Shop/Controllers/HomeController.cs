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

            // Chỉ định ID của các sản phẩm tiêu biểu bạn muốn hiển thị
            List<int> featuredProductIds = new List<int> { 85, 88, 94, 300, 564, 654, 237, 78 };

            List<Product> featuredProducts = new List<Product>();
            foreach (var id in featuredProductIds)
            {
                var product = await CatalogDataService.GetProductAsync(id);
                if (product != null)
                {
                    featuredProducts.Add(product);
                }
            }

            // Nếu không tìm thấy sản phẩm nào theo ID chỉ định, 
            // fallback về cách lấy danh sách mặc định
            if (featuredProducts.Count == 0)
            {
                var productResult = await CatalogDataService.ListProductsAsync(new ProductSearchInput
                {
                    Page = 1,
                    PageSize = 8,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                });
                featuredProducts = productResult.DataItems;
            }

            return View(featuredProducts);
        }
        public IActionResult Contact()
        {
            return View("Contact");
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
