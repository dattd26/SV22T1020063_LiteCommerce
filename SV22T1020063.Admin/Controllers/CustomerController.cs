using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Partner;

namespace SV22T1020063.Admin.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        private const string CUSTOMER_SEARCH = "CustomerSearchInput";
        /// <summary>
        /// Nhập đầu tìm kiếm, Hiển thị kết quả tìm kiếm
        /// </summary>
        /// <param name="page"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
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
        /// <summary>
        /// Tìm kiếm và trả về kết quả
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListCustomersAsync(input);
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            var model = new Customer()
            {
                CustomerID = 0
            };

            return View("Edit", model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Customer data)
        {
            try
            {
                /*throw new Exception();*/
                ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";

                // TODO: Kiểm tra dữ liệu đầu vào có hợp lệ không

                // Sử dụng ModelState để lưu trữ các tình huống lỗi (thông báo) lỗi thông báo cho View
                // Giả thiết: chỉ cần nhậpt tên, email và tỉnh thành
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError(nameof(data.CustomerName), "Vui lòng nhập tên khách hàng");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
                else if (!await PartnerDataService.ValidatelCustomerEmailAsync(data.Email, data.CustomerID))
                    ModelState.AddModelError(nameof(data.Email), "Email này đã được sử dụng");
                if (string.IsNullOrEmpty(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");

                //(Tùy chọn) Hiệu chỉnh dữ liệu theo qui tắt của phần mềm
                if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = data.CustomerName;
                if (string.IsNullOrEmpty(data.Phone)) data.Phone = string.Empty;
                if (string.IsNullOrEmpty(data.Address)) data.Address = string.Empty;

                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                // Lưu vào CSDL
                if (data.CustomerID == 0)
                {
                    await PartnerDataService.AddCustomerAsync(data);
                }
                else
                {
                    await PartnerDataService.UpdateCustomerAsync(data);
                }
                return RedirectToAction("Edit");
            }
            catch //(Exception ex)
            {
                // Ghi lại log lỗi ex.Message, ex.StackTrace
                ModelState.AddModelError("Error", "Hệ thống tạm thời đang bận, vui lòng thử lại sau");
                return View("Edit", data);
            }


        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    await PartnerDataService.DeleteCustomerAsync(id);
                    return RedirectToAction("Index");
                }
                // GET
                var model = await PartnerDataService.GetCustomerAsync(id);
                if (model == null)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.CanDelete = !await PartnerDataService.IsUsedCustomerAsync(id);

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không xác định");
                return RedirectToAction("Index");
            }
        }

        /*[HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        */

        public IActionResult ChangePassword(int id)
        {
            return View();
        }
    }
}
