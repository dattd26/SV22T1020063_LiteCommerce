using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Sales;
using SV22T1020063.Models.Catalog;
using SV22T1020063.Admin.Models;

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
            var result = await SalesDataService.ListOrdersAsync(input);
            return View(result);
        }

        // Tạo đơn hàng mới (Giao diện giỏ hàng)
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>("ProductSearchForSale");
            if (input == null)
            {
                input = new ProductSearchInput() { Page = 1, PageSize = 8, SearchValue = "" };
            }
            return View(input);
        }

        public async Task<IActionResult> SearchProduct(ProductSearchInput input)
        {
            input.PageSize = 8;
            var result = await ProductDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData("ProductSearchForSale", input);
            return View(result);
        }

        private const string SHOPPING_CART = "ShoppingCart";

        private List<CartItem> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (cart == null)
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return cart;
        }

        public IActionResult AddToCart(CartItem item)
        {
            if (item.Quantity <= 0 || item.SalePrice < 0)
                return Json("Dữ liệu không hợp lệ");

            var cart = GetShoppingCart();
            var existsItem = cart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsItem == null)
            {
                cart.Add(item);
            }
            else
            {
                existsItem.Quantity += item.Quantity;
                existsItem.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return View("GetCart", cart);
        }

        public IActionResult RemoveFromCart(int id = 0)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                cart.RemoveAt(index);

            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return View("GetCart", cart);
        }

        public IActionResult ClearCart()
        {
            var cart = GetShoppingCart();
            cart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return View("GetCart", cart);
        }

        public IActionResult GetCart()
        {
            var cart = GetShoppingCart();
            return View(cart);
        }
        // Xem chi tiết đơn hàng (và thực hiện các thao tác chuyển trạng thái)
        public IActionResult Detail(int id) => View();

        /// <summary>
        /// Khởi tạo đơn hàng (lưu vào database)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Init(int customerID, string deliveryProvince, string deliveryAddress)
        {
            var cart = GetShoppingCart();
            if (cart.Count == 0)
                return Json("Giỏ hàng trống");

            if (customerID <= 0 || string.IsNullOrEmpty(deliveryProvince) || string.IsNullOrEmpty(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");

            // TODO: Lấy ID nhân viên đang đăng nhập. Tạm thời lấy ID = 1
            int employeeID = 1;

            Order order = new Order()
            {
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                EmployeeID = employeeID,
                Status = OrderStatusEnum.New
            };
            int orderID = await SalesDataService.AddOrderAsync(order);

            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail()
                {
                    OrderID = orderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                };
                await SalesDataService.AddDetailAsync(detail);
            }

            cart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);

            return Json(new { success = true, orderID = orderID });
        }

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