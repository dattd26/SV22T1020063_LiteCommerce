using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.DataLayers.SQLServer;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Sales;

namespace SV22T1020063.BusinessLayers
{
    /// <summary>
    /// Cung cấp các tính năng xử lý dữ liệu liên quan đến bán hàng (đơn hàng)
    /// </summary>
    public static class SaleDataService
    {
        private static readonly IOrderRepository orderDB;

        static SaleDataService()
        {
            orderDB = new OrderRepository(Configuration.ConnectionString);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách đơn hàng dưới dạng phân trang
        /// </summary>
        public static async Task<PagedResult<OrderViewInfo>> ListOrdersAsync(OrderSearchInput input)
        {
            return await orderDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết một đơn hàng
        /// </summary>
        public static async Task<OrderViewInfo?> GetOrderAsync(int orderID)
        {
            return await orderDB.GetAsync(orderID);
        }

        /// <summary>
        /// Bổ sung một đơn hàng mới
        /// </summary>
        public static async Task<int> AddOrderAsync(Order data)
        {
            return await orderDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin nhận hàng của đơn hàng
        /// </summary>
        public static async Task<bool> UpdateOrderAsync(Order data)
        {
            return await orderDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        public static async Task<bool> DeleteOrderAsync(int orderID)
        {
            return await orderDB.DeleteAsync(orderID);
        }

        /// <summary>
        /// Lấy danh sách mặt hàng của đơn hàng
        /// </summary>
        public static async Task<List<OrderDetailViewInfo>> ListOrderDetailsAsync(int orderID)
        {
            return await orderDB.ListDetailsAsync(orderID);
        }

        /// <summary>
        /// Lấy thông tin một mặt hàng trong đơn hàng
        /// </summary>
        public static async Task<OrderDetailViewInfo?> GetOrderDetailAsync(int orderID, int productID)
        {
            return await orderDB.GetDetailAsync(orderID, productID);
        }

        /// <summary>
        /// Lưu/cập nhật chi tiết đơn hàng
        /// </summary>
        public static async Task<bool> SaveOrderDetailAsync(OrderDetail data)
        {
            var detail = await orderDB.GetDetailAsync(data.OrderID, data.ProductID);
            if (detail == null)
                return await orderDB.AddDetailAsync(data);
            else
                return await orderDB.UpdateDetailAsync(data);
        }

        /// <summary>
        /// Xóa một mặt hàng khỏi đơn hàng
        /// </summary>
        public static async Task<bool> DeleteOrderDetailAsync(int orderID, int productID)
        {
            return await orderDB.DeleteDetailAsync(orderID, productID);
        }
    }
}
