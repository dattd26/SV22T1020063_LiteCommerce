using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Sales;
using SV22T1020063.Shop.AppCodes;

namespace SV22T1020063.Shop.Controllers
{
    [Authorize(Roles = WebUserRoles.Customer)]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity, decimal salePrice)
        {
            if (productId <= 0 || quantity <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            var product = await CatalogDataService.GetProductAsync(productId);
            if (product == null || !product.IsSelling)
            {
                return Json(new { success = false, message = "Sản phẩm không hợp lệ hoặc đã ngừng kinh doanh" });
            }

            var actualSalePrice = salePrice > 0 ? salePrice : product.Price;

            ShoppingCartService.AddCartItem(new OrderDetailViewInfo()
            {
                ProductID = productId,
                Quantity = quantity,
                SalePrice = actualSalePrice,
                ProductName = product.ProductName,
                Photo = string.IsNullOrEmpty(product.Photo) ? "noPhoto.png" : product.Photo,
                Unit = product.Unit
            });

            return Json(new { success = true, itemsCount = ShoppingCartService.GetShoppingCart().Count });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            ShoppingCartService.RemoveCartItem(productId);
            return Json(new { success = true, itemsCount = ShoppingCartService.GetShoppingCart().Count });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                ShoppingCartService.RemoveCartItem(productId);
            }
            else
            {
                var item = ShoppingCartService.GetCartItem(productId);
                if (item != null)
                {
                    ShoppingCartService.UpdateCartItem(productId, quantity, item.SalePrice);
                }
            }
            return Json(new { success = true, itemsCount = ShoppingCartService.GetShoppingCart().Count });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCartInfo()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return Json(new { itemsCount = cart.Count, total = cart.Sum(x => x.Quantity * x.SalePrice) });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCartSidebar()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return PartialView("_CartSidebar", cart);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            if (cart.Count == 0)
                return RedirectToAction("Index", "Home");

            int customerId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var customer = await PartnerDataService.GetCustomerAsync(customerId);
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            return View(customer);
        }

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    CustomerID = int.Parse(User.FindFirst("UserId")?.Value ?? "0")
                };
            }
            return View(condition);
        }

        public async Task<IActionResult> Search(OrderSearchInput input)
        {
            input.CustomerID = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            input.PageSize = PAGE_SIZE;
            var result = await SalesDataService.ListOrdersAsync(input);

            foreach (var order in result.DataItems)
            {
                order.Details = await SalesDataService.ListDetailsAsync(order.OrderID);
            }

            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, input);
            return PartialView("_OrderList", result);
        }

        public IActionResult Success()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            int customerId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (order.CustomerID != customerId)
            {
                return RedirectToAction("Index");
            }

            order.Details = await SalesDataService.ListDetailsAsync(id);
            return View(order);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");

            int customerId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (order.CustomerID != customerId) return RedirectToAction("Index");

            await SalesDataService.CancelOrderAsync(id);
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> Init(string deliveryProvince, string deliveryAddress)
        {
            var cart = ShoppingCartService.GetShoppingCart();
            if (cart.Count == 0)
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống" });

            if (string.IsNullOrEmpty(deliveryProvince) || string.IsNullOrEmpty(deliveryAddress))
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin giao hàng" });

            int customerId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            Order data = new Order()
            {
                CustomerID = customerId,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                EmployeeID = null, // Shop orders don't have employee initially
                Status = OrderStatusEnum.New
            };

            int orderId = await SalesDataService.AddOrderAsync(data);
            if (orderId > 0)
            {
                foreach (var item in cart)
                {
                    await SalesDataService.AddDetailAsync(new OrderDetail()
                    {
                        OrderID = orderId,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        SalePrice = item.SalePrice
                    });
                }
                ShoppingCartService.ClearCart();
                return Json(new { success = true, orderId });
            }

            return Json(new { success = false, message = "Không thể khởi tạo đơn hàng. Vui lòng thử lại sau" });
        }
    }
}
