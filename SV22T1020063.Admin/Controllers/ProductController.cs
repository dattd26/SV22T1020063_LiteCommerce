using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Catalog;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Admin.Controllers
{
    public class ProductController : Controller
    {
        private const int PAGESIZE = 10;
        private const string PRODUCT_SEARCH = "ProductSearch";

        public async Task<IActionResult> Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null || (searchValue != "" || categoryID != 0 || supplierID != 0 || minPrice != 0 || maxPrice != 0 || page != 1))
            {
                input = new ProductSearchInput
                {
                    Page = page,
                    PageSize = PAGESIZE,
                    SearchValue = searchValue ?? "",
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
            }

            // Lưu lại bộ lọc vào Session
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            // Gán giá trị cho ViewBag để hiển thị lại trên Form
            ViewBag.SearchValue = input.SearchValue;
            ViewBag.CategoryID = input.CategoryID;
            ViewBag.SupplierID = input.SupplierID;
            ViewBag.MinPrice = input.MinPrice > 0 ? input.MinPrice.ToString() : "";
            ViewBag.MaxPrice = input.MaxPrice > 0 ? input.MaxPrice.ToString() : "";

            // Lấy danh sách Loại hàng và Nhà cung cấp để hiển thị trên Filter
            var catResult = await CategoryDataService.ListCategoryAsync(new PaginationSearchInput { PageSize = 0 });
            ViewBag.Categories = catResult.DataItems;

            var supResult = await PartnerDataService.ListSupplierAsync(new PaginationSearchInput { PageSize = 0 });
            ViewBag.Suppliers = supResult.DataItems;

            var result = await ProductDataService.ListProductsAsync(input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            return View("Edit");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa mặt hàng";
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            return View();
        }
        public IActionResult ListAttributes(int pid)
        {
            return View();
        }
        public IActionResult CreateAttribute(int pid)
        {
            return View();
        }
        public IActionResult EditAttribute(int pid, int attributeId)
        {
            return View();
        }
        public IActionResult DeleteAttribute(int pid, int attributeId)
        {
            return RedirectToAction("Edit", new { pid });
        }
        public IActionResult ListPhotos(int pid)
        {
            return View();
        }
        public IActionResult CreatePhoto(int pid)
        {
            ViewBag.Title = "Bổ sung ảnh";
            return View();
        }
        public IActionResult EditPhoto(int pid, int photoId)
        {

            ViewBag.Title = "Cập nhật ảnh";
            return View();
        }

        public IActionResult DeletePhoto(int pid, int photoId)
        {
            return RedirectToAction("Edit", new { pid });
        }
    }
}
