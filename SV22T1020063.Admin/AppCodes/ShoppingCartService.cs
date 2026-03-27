using SV22T1020063.Admin;
using SV22T1020063.Models.Sales;

namespace SV22T1020063.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý trên giỏ hàng
    /// (Giỏ hàng lưu trong session)
    /// </summary>
    public static class ShoppingCartService
    {
        /// <summary>
        /// Tên biến để lưu giỏ hàng trong session
        /// </summary>
        private const string CART = "ShoppingCart";
        // Lấy giỏ hàng từ session
        public static List<OrderDetailViewInfo> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<OrderDetailViewInfo>>(CART);
            if (cart == null)
            {
                cart = new List<OrderDetailViewInfo>();

            }
            return cart;
        }
        // Lấy thôgn tin 1 mặt hàng từ giỏ hàng
        public static OrderDetailViewInfo? GetCartItem(int productID)
        {
            var cart = GetShoppingCart();
            return cart.Find(m => m.ProductID == productID);

        }
        // Thêm hàng vào giỏ hàng
        public static void AddCartItem(OrderDetailViewInfo item)
        {
            var cart = GetShoppingCart();
            var existsItem = cart.Find(m => m.ProductID == item.ProductID);
            if (existsItem == null)
            {
                cart.Add(item);
            }
            else
            {
                existsItem.Quantity += item.Quantity;
                existsItem.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(CART, cart);
        }
        // Cập nhật số lượng và  giá của một mặt hàng trong giỏ hàng
        public static void UpdateCartItem(int productID, int quantity, decimal salePrice)
        {
            var cart = GetShoppingCart();
            var item = cart.Find(m => m.ProductID == productID);
            if (item != null)
            {
                item.Quantity = quantity;
                item.SalePrice = salePrice;
                ApplicationContext.SetSessionData(CART, cart);
            }
        }
        // Xóa một mặt hàng ra khỏi giỏ hàng
        public static void RemoveCartItem(int productID)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == productID);
            if (index >= 0)
            {
                cart.RemoveAt(index);
                ApplicationContext.SetSessionData(CART, cart);
            }
        }
        // Xóa giỏ hàng
        public static void ClearCart()
        {
            var cart = new List<OrderDetailViewInfo>();
            ApplicationContext.SetSessionData(CART, cart);
        }

    }
}
