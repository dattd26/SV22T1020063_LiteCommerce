using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Partner;

namespace SV22T1020063.Admin.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private const string SUPPLIER_SEARCH = "SupplierSearchInput";
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
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

            var result = await PartnerDataService.ListSuppliersAsync(input);
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            var model = new Supplier()
            {
                SupplierID = 0
            };

            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Supplier data)
        {
            try
            {
                ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";

                //Kiểm tra dữ liệu đầu vào: FullName và Email là bắt buộc, Email chưa được sử dụng bởi nhân viên khác
                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError(nameof(data.SupplierName), "Vui lòng nhập tên nhà cung cấp");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Email không được để trống");

                if (string.IsNullOrEmpty(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");

                //(Tùy chọn) Hiệu chỉnh dữ liệu theo qui tắt của phần mềm
                if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = data.SupplierName;
                if (string.IsNullOrEmpty(data.Phone)) data.Phone = string.Empty;
                if (string.IsNullOrEmpty(data.Address)) data.Address = string.Empty;


                //Lưu dữ liệu vào database (bổ sung hoặc cập nhật)
                if (data.SupplierID == 0)
                {
                    await PartnerDataService.AddSupplierAsync(data);
                }
                else
                {
                    await PartnerDataService.UpdateSupplierAsync(data);
                }
                return RedirectToAction("Index");
            }
            catch //(Exception ex)
            {
                //TODO: Ghi log lỗi căn cứ vào ex.Message và ex.StackTrace
                ModelState.AddModelError(string.Empty, "Hệ thống đang bận hoặc dữ liệu không hợp lệ. Vui lòng kiểm tra dữ liệu hoặc thử lại sau");
                return View("Edit", data);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await PartnerDataService.GetSupplierAsync(id);
            if (model == null)
            {
                ModelState.AddModelError(nameof(model), "Nhà cung cấp không tồn tại");
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Chỉnh sửa nhà cung cấp";
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    await PartnerDataService.DeleteSupplierAsync(id);
                    return RedirectToAction("Index");
                }

                // GET
                var model = await PartnerDataService.GetSupplierAsync(id);
                if (model == null)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.CanDelete = !await PartnerDataService.IsUsedSupplierAsync(id);

                return View(model);
            }
            catch //(Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không xác định");
                return RedirectToAction("Index");
            }
        }

    }
}
