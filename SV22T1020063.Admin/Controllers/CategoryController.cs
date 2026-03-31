using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Admin.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private const string CATEGORY_SEARCH = "CategorySearchInput";
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
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
            
            var result = await CatalogDataService.ListCategoriesAsync(input);
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            return View("Edit");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa loại hàng";
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
