using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Catalog;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Admin.Controllers
{
    public class ProductController : Controller
    {
        private const string PRODUCT_SEARCH = "ProductSearchInput";
        public async Task<IActionResult> Index()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = string.Empty,
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput())).DataItems;
            ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput())).DataItems;
            return View(input);
        }
        public async Task<IActionResult> Search(ProductSearchInput input) {
            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
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
