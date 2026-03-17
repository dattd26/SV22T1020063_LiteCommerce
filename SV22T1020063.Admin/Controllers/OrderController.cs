using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Sales;
using SV22T1020063.Models.Catalog;

namespace SV22T1020063.Admin.Controllers
{
    public class OrderController : Controller
    {
        private const int PAGESIZE = 10;

        // Tra cứu đơn hàng
        public IActionResult Index() => View();

        // Tìm kiếm (thường trả về danh sách kết quả qua AJAX)
        public async Task<IActionResult> Search(OrderSearchInput input)
        {
            input.PageSize = PAGESIZE;
            var result = await SaleDataService.ListOrdersAsync(input);
            return View(result);
        }

        // Tạo đơn hàng mới (Giao diện giỏ hàng)
        public async Task<IActionResult> Create(int page = 1, string searchValue = "")
        {
            var result = await ProductDataService.ListProductsAsync(new ProductSearchInput { Page = page, PageSize = PAGESIZE, SearchValue = searchValue, CategoryID = 0, SupplierID = 0, MinPrice = 0, MaxPrice = 0 });
            ViewBag.SearchValue = searchValue;
            return View(result);
        }
        // Xem chi tiết đơn hàng (và thực hiện các thao tác chuyển trạng thái)
        public IActionResult Detail(int id) => View();

        // Các thao tác với giỏ hàng khi đang tạo đơn mới
        /// <summary>
        /// Cập nhật thông tin (số lượng, giá bán) của một mặt hàng trong giỏ hàng hoặc trong một đơn hàng
        /// </summary>
        /// <param name="id">0:  Cập nhật cho Giỏ hàng, khác 0: Cập nhật cho đơn hàng</param>
        /// <param name="productId">Mã hàng</param>
        /// <returns></returns>
        public IActionResult EditCartItem(int id = 0, int productId = 0)
        {
            if (id == 0)
            {
                // Xử lý cho giỏ hàng
            }
            else
            {
                // Xử lý cho đơn hàng
            }
            return View();
        }
        /// <summary>
        /// Xóa mặt hàng ra khỏi giỏ hàng hoặc đơn hàng
        /// </summary>
        /// <param name="id">0: Xóa mặt hàng khỏi giỏ, khác 0: Xóa mặt hàng khỏi đơn hàng có mã pid</param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult DeleteCartItem(int id = 0, int productId = 0)
        {

            return PartialView();
        }
        public IActionResult ClearCart() => RedirectToAction("Create");

        // Các thao tác chuyển trạng thái đơn hàng
        /// <summary>
        /// Duyệt đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Accept(int id) => RedirectToAction("Detail", new { id });
        /// <summary>
        /// Chuyển đơn hàng cho người giao hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng cần xử lý</param>
        /// <returns></returns>
        public IActionResult Shipping(int id) => RedirectToAction("Detail", new { id });
        /// <summary>
        /// Ghi nhận đơn hàng kết thúc thành công
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id) => RedirectToAction("Detail", new { id });
        public IActionResult Reject(int id) => RedirectToAction("Detail", new { id });
        public IActionResult Cancel(int id) => RedirectToAction("Detail", new { id });
        public IActionResult Delete(int id) => RedirectToAction("Index");
    }
}