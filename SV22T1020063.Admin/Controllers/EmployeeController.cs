using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;


namespace SV22T1020063.Admin.Controllers
{
    public class EmployeeController : Controller
    {

        private const string EMPLOYEE_SEARCH = "EmployeeSearchInput";
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await HRDataService.ListEmployeesAsync(input);
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            return View("Edit");
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa nhân viên";
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
        public IActionResult ChangeRole(int id)
        {
            return View();
        }
    }
}
