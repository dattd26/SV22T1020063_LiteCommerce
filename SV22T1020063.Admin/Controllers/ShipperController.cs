using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Partner;

namespace SV22T1020063.Admin.Controllers
{
    public class ShipperController : Controller
    {
        private const string SHIPPER_SEARCH = "ShipperSearchInput";
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
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
            
            var result = await PartnerDataService.ListShippersAsync(input);
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            var model = new Shipper() {
                ShipperID = 0
            };

            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Shipper data)
        {
            try
            {
                ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";

                //Kiểm tra dữ liệu đầu vào: FullName và Email là bắt buộc, Email chưa được sử dụng bởi nhân viên khác
                if (string.IsNullOrWhiteSpace(data.ShipperName))
                    ModelState.AddModelError(nameof(data.ShipperName), "Vui lòng nhập tên người giao hàng");

                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");

                if (!ModelState.IsValid)
                    return View("Edit", data);

                //Lưu dữ liệu vào database (bổ sung hoặc cập nhật)
                if (data.ShipperID == 0)
                {
                    await PartnerDataService.AddShipperAsync(data);
                }
                else
                {
                    await PartnerDataService.UpdateShipperAsync(data);
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
            var model = await PartnerDataService.GetShipperAsync(id);
            if (model == null)
            {
                ModelState.AddModelError(nameof(model), "Người giao hàng không tồn tại");
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Chỉnh sửa người giao hàng";
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    await PartnerDataService.DeleteShipperAsync(id);
                    return RedirectToAction("Index");
                }
                
                // GET
                var model = await PartnerDataService.GetShipperAsync(id);
                if (model == null)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.CanDelete = !await PartnerDataService.IsUsedShipperAsync(id);

               return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không xác định");
                return RedirectToAction("Index");
            }
        }
    }
}
