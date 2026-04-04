using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Catalog;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Shop.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> Index(int categoryID = 0, string searchValue = "")
        {
            ViewBag.Title = "Tất cả sản phẩm";
            ViewBag.CategoryID = categoryID;
            ViewBag.SearchValue = searchValue;

            var categories = await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput
            {
                Page = 1,
                PageSize = 100,
                SearchValue = ""
            });
            ViewBag.Categories = categories.DataItems;

            return View();
        }

        public async Task<IActionResult> Search(ProductSearchInput input)
        {
            int pageSize = 12;
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
    }
}
