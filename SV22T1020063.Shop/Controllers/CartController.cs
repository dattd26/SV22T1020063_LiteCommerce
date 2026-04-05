using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Sales;
using SV22T1020063.Shop.AppCodes;

namespace SV22T1020063.Shop.Controllers
{
    public class CartController : Controller
    {
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

            return Json(new { success = true, itemsCount = ShoppingCartService.GetShoppingCart().Sum(x => x.Quantity) });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            ShoppingCartService.RemoveCartItem(productId);
            return Json(new { success = true, itemsCount = ShoppingCartService.GetShoppingCart().Sum(x => x.Quantity) });
        }

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
            return Json(new { success = true, itemsCount = ShoppingCartService.GetShoppingCart().Sum(x => x.Quantity) });
        }

        [HttpGet]
        public IActionResult GetCartInfo()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return Json(new { itemsCount = cart.Sum(x => x.Quantity), total = cart.Sum(x => x.Quantity * x.SalePrice) });
        }

        [HttpGet]
        public IActionResult GetCartSidebar()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return PartialView("_CartSidebar", cart);
        }
    }
}
