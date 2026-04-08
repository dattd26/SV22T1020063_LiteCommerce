using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Sales;
using SV22T1020063.Models.Catalog;
using SV22T1020063.Models.Common;
using SV22T1020063.Admin.Models;
using SV22T1020063.Admin.AppCodes;
using Microsoft.AspNetCore.Authorization;

namespace SV22T1020063.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Sales}")]
    public class OrderController : Controller
    {
        private const int PAGESIZE = 10;

        private const string PRODUCT_SEARCH = "SearchProductSale";
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
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput() { Page = 1, PageSize = 5, SearchValue = "" };
            }
            return View(input);
        }

        public async Task<IActionResult> SearchProduct(ProductSearchInput input)
        {
            input.PageSize = 5;
            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(result);
        }




        public async Task<IActionResult> AddCartItem(int productID, int quantity, decimal price)
        {
            if (productID <= 0 || quantity <= 0 || price <= 0)
            {
                return Json(new ApiResult(0, "Dữ liệu không hợp lệ"));
            }
            var product = await CatalogDataService.GetProductAsync(productID);
            if (product == null)
            {
                return Json(new ApiResult(0, "Mặt hàng không tồn tại"));
            }
            if (!product.IsSelling)
            {
                return Json(new ApiResult(0, "Mặt hàng đã ngừng bán"));
            }
            ShoppingCartService.AddCartItem(new OrderDetailViewInfo()
            {
                ProductID = productID,
                Quantity = quantity,
                SalePrice = price,
                ProductName = product.ProductName,
                Photo = string.IsNullOrEmpty(product.Photo) ? "noPhoto.png" : product.Photo,
                Unit = product.Unit,
            });
            return Json(new ApiResult(1, "Thêm vào giỏ hàng thành công"));
        }

        public IActionResult EditCartItem(int id, int quantity, decimal salePrice)
        {
            if (Request.Method == "POST")
            {
                ShoppingCartService.UpdateCartItem(id, quantity, salePrice);
                return Json(new ApiResult(1, "Cập nhật giỏ hàng thành công"));
            }

            var item = ShoppingCartService.GetCartItem(id);
            if (item == null)
                return Json(new ApiResult(0, "Không tìm thấy mặt hàng này trong giỏ hàng"));
            return PartialView("EditCartItem", item);
        }


        public IActionResult DeleteCartItem(int id)
        {
            if (Request.Method == "POST")
            {
                ShoppingCartService.RemoveCartItem(id);
                return Json(new ApiResult(1, "Xóa mặt hàng thành công"));
            }

            var item = ShoppingCartService.GetCartItem(id);
            if (item == null)
                return Json(new ApiResult(0, "Không tìm thấy mặt hàng này trong giỏ hàng"));
            return PartialView("DeleteCartItem", item);
        }

        public IActionResult ClearCart()
        {
            ShoppingCartService.ClearCart();
            return RedirectToAction("ShowCart");
        }

        public IActionResult ShowCart()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return View(cart);
        }

        // Xem chi tiết đơn hàng (và thực hiện các thao tác chuyển trạng thái)
        public async Task<IActionResult> Detail(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null)
                return RedirectToAction("Index");

            order.Details = await SalesDataService.ListDetailsAsync(id);
            foreach (var detail in order.Details)
            {
                var product = await CatalogDataService.GetProductAsync(detail.ProductID);
                if (product != null)
                {
                    detail.ProductName = product.ProductName;
                    detail.Photo = string.IsNullOrEmpty(product.Photo) ? "noPhoto.png" : product.Photo;
                    detail.Unit = product.Unit;
                }
            }
            return View(order);
        }

        /// <summary>
        /// Khởi tạo đơn hàng (lưu vào database)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Init(int customerID, string deliveryProvince, string deliveryAddress)
        {
            var cart = ShoppingCartService.GetShoppingCart();
            if (cart.Count == 0)
                return Json(new ApiResult(0, "Giỏ hàng trống"));

            if (customerID <= 0 || string.IsNullOrEmpty(deliveryProvince) || string.IsNullOrEmpty(deliveryAddress))
                return Json(new ApiResult(0, "Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng"));

            var userData = User.GetUserData();
            if (userData == null) {
                return Json(new ApiResult(0, "Không xác định được thông tin nhan vien"));
            }

            int employeeID = 0;
            
            try
            {
                employeeID = int.Parse(userData.UserId);
                if (employeeID <= 0)
                {
                    return Json(new ApiResult(0, "Không xác định được thông tin nhan vien"));
                }
            }
            catch (Exception ex) {
                return Json(new ApiResult(0, "Ma nhân viên không hợp lệ"));
            }


            Order order = new Order()
            {
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                EmployeeID = employeeID,
                Status = OrderStatusEnum.New
            };

            int orderID = await SalesDataService.AddOrderAsync(customerID, deliveryProvince, deliveryAddress, employeeID);

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

            ShoppingCartService.ClearCart();

            return Json(new ApiResult(1, orderID.ToString()));
        }

        // Các thao tác chuyển trạng thái đơn hàng
        /// <summary>
        /// Duyệt đơn hàng
        /// </summary>
        public async Task<IActionResult> Accept(int id)
        {
            if (Request.Method == "POST")
            {
                await SalesDataService.AcceptOrderAsync(id, 1); // Tạm thời ID nhân viên = 1
                return RedirectToAction("Detail", new { id });
            }
            return View(id);
        }

        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        public async Task<IActionResult> Reject(int id)
        {
            if (Request.Method == "POST")
            {
                await SalesDataService.RejectOrderAsync(id, 1); 
                return RedirectToAction("Detail", new { id });
            }
            return View(id);
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        public async Task<IActionResult> Cancel(int id)
        {
            if (Request.Method == "POST")
            {
                await SalesDataService.CancelOrderAsync(id);
                return RedirectToAction("Detail", new { id });
            }
            return View(id);
        }

        /// <summary>
        /// Ghi nhận đơn hàng kết thúc thành công
        /// </summary>
        public async Task<IActionResult> Finish(int id)
        {
            if (Request.Method == "POST")
            {
                await SalesDataService.CompleteOrderAsync(id);
                return RedirectToAction("Detail", new { id });
            }
            return View(id);
        }

        /// <summary>
        /// Chuyển đơn hàng cho người giao hàng
        /// </summary>
        public async Task<IActionResult> Shipping(int id, int shipperID = 0)
        {
            Console.WriteLine(shipperID);
            if (Request.Method == "POST")
            {
                if (shipperID == 0)
                {
                    ModelState.AddModelError("shipperID", "Vui lòng chọn người giao hàng");
                    return RedirectToAction("Detail", new { id });
                }
                else
                {
                    await SalesDataService.ShipOrderAsync(id, shipperID);
                    return RedirectToAction("Detail", new { id });
                }
            }
            ViewBag.OrderID = id;
            var input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 100,
                SearchValue = ""
            };
            var result = await PartnerDataService.ListShippersAsync(input);
            return View(result);
        }

        public async Task<IActionResult> SearchShipper(int id, string searchValue = "")
        {
            var input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 100,
                SearchValue = searchValue ?? ""
            };
            var result = await PartnerDataService.ListShippersAsync(input);
            ViewBag.OrderID = id;
            return View(result);
        }

        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await SalesDataService.DeleteOrderAsync(id);
                return RedirectToAction("Index");
            }
            return View(id);
        }
    }
}