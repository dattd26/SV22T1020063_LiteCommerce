
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.DataLayers.SQLServer;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Partner;

namespace SV22T1020063.BusinessLayers
{
    /// <summary>
    /// Cung cấp các tính năng xử lý dữ liệu liên quan đến đối tác của hệ thống
    /// Bao gồm: Supplier, Customer, Shipper
    /// </summary>
    public static class PartnerDataService
    {
        private static readonly IGenericRepository<Supplier> supplierDB;
        private static readonly IGenericRepository<Shipper> shipperDB;
        private static readonly ICustomerRepository customerDB;

        static PartnerDataService()
        {
            supplierDB = new SupplierRepository(Configuration.ConnectionString);
            shipperDB = new ShipperRepository(Configuration.ConnectionString);
            customerDB = new CustomerRepository(Configuration.ConnectionString);
        }

        //= Các chức năng liên quan đến nhà cung cấp
        /// <summary>
        /// Tìm kiếm và trả về danh sách nhà cung cấp dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách nhà cung cấp dạng phân trang</returns>
        public static async Task<PagedResult<Supplier>> ListSupplierAsync(PaginationSearchInput input)
        {
            return await supplierDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một nhà cung cấp có mã là <paramref name="supplierId"/>
        /// </summary>
        /// <param name="supplierId">Mã nhà cung cấp cần lấy thông tin</param>
        /// <returns>Thông tin nhà cung cấp nếu tồn tại, ngược lại trả về null</returns>
        public static async Task<Supplier?> GetSupplierAsync(int supplierId)
        {
            return await supplierDB.GetAsync(supplierId);
        }

        /// <summary>
        /// Bổ sung một nhà cung cấp mới
        /// </summary>
        /// <param name="supplier">Thông tin nhà cung cấp cần bổ sung</param>
        /// <returns>Mã nhà cung cấp được tạo</returns>
        public static async Task<int> AddSupplierAsync(Supplier supplier)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            return await supplierDB.AddAsync(supplier);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhà cung cấp
        /// </summary>
        /// <param name="supplier">Thông tin nhà cung cấp cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, ngược lại False</returns>
        public static async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi cập nhật
            return await supplierDB.UpdateAsync(supplier);
        }

        /// <summary>
        /// Xóa nhà cung cấp có mã là <paramref name="supplierId"/>
        /// </summary>
        /// <param name="supplierId">Mã nhà cung cấp cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu không thể xóa</returns>
        public static async Task<bool> DeleteSupplierAsync(int supplierId)
        {
            if (await supplierDB.IsUsedAsync(supplierId))
            {
                return false;
            }
            return await supplierDB.DeleteAsync(supplierId);
        }

        /// <summary>
        /// Kiểm tra xem một nhà cung cấp có mặt hàng liên quan không
        /// (dùng để kiểm tra xem có được phép xóa hay không)
        /// </summary>
        /// <param name="supplierId">Mã nhà cung cấp</param>
        /// <returns>True nếu đang được sử dụng, False nếu không</returns>
        public static async Task<bool> IsUsedSupplierAsync(int supplierId)
        {
            return await supplierDB.IsUsedAsync(supplierId);
        }

        //= Các chức năng liên quan đến người giao hàng

        /// <summary>
        /// Tìm kiếm và trả về danh sách người giao hàng dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách người giao hàng dạng phân trang</returns>
        public static async Task<PagedResult<Shipper>> ListShipperAsync(PaginationSearchInput input)
        {
            return await shipperDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một người giao hàng có mã là <paramref name="shipperId"/>
        /// </summary>
        /// <param name="shipperId">Mã người giao hàng</param>
        /// <returns>Thông tin người giao hàng nếu tồn tại, ngược lại null</returns>
        public static async Task<Shipper?> GetShipperAsync(int shipperId)
        {
            return await shipperDB.GetAsync(shipperId);
        }

        /// <summary>
        /// Bổ sung một người giao hàng mới
        /// </summary>
        /// <param name="shipper">Thông tin người giao hàng</param>
        /// <returns>Mã người giao hàng được tạo</returns>
        public static async Task<int> AddShipperAsync(Shipper shipper)
        {
            // TODO: Kiểm tra dữ liệu trước khi thêm
            return await shipperDB.AddAsync(shipper);
        }

        /// <summary>
        /// Cập nhật thông tin người giao hàng
        /// </summary>
        /// <param name="shipper">Thông tin người giao hàng cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công</returns>
        public static async Task<bool> UpdateShipperAsync(Shipper shipper)
        {
            // TODO: Kiểm tra dữ liệu trước khi cập nhật
            return await shipperDB.UpdateAsync(shipper);
        }

        /// <summary>
        /// Xóa người giao hàng có mã là <paramref name="shipperId"/>
        /// </summary>
        /// <param name="shipperId">Mã người giao hàng</param>
        /// <returns>True nếu xóa thành công</returns>
        public static async Task<bool> DeleteShipperAsync(int shipperId)
        {
            if (await shipperDB.IsUsedAsync(shipperId))
            {
                return false;
            }
            return await shipperDB.DeleteAsync(shipperId);
        }

        /// <summary>
        /// Kiểm tra xem người giao hàng có dữ liệu liên quan không
        /// </summary>
        /// <param name="shipperId">Mã người giao hàng</param>
        /// <returns>True nếu đang được sử dụng</returns>
        public static async Task<bool> IsUsedShipperAsync(int shipperId)
        {
            return await shipperDB.IsUsedAsync(shipperId);
        }

        //= Các chức năng liên quan đến khách hàng

        /// <summary>
        /// Tìm kiếm và trả về danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách khách hàng dạng phân trang</returns>
        public static async Task<PagedResult<Customer>> ListCustomerAsync(PaginationSearchInput input)
        {
            return await customerDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một khách hàng có mã là <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">Mã khách hàng</param>
        /// <returns>Thông tin khách hàng nếu tồn tại, ngược lại null</returns>
        public static async Task<Customer?> GetCustomerAsync(int customerId)
        {
            return await customerDB.GetAsync(customerId);
        }

        /// <summary>
        /// Bổ sung một khách hàng mới
        /// </summary>
        /// <param name="customer">Thông tin khách hàng</param>
        /// <returns>Mã khách hàng được tạo</returns>
        public static async Task<int> AddCustomerAsync(Customer customer)
        {
            // TODO: Kiểm tra dữ liệu trước khi thêm
            return await customerDB.AddAsync(customer);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="customer">Thông tin khách hàng cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công</returns>
        public static async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            // TODO: Kiểm tra dữ liệu trước khi cập nhật
            return await customerDB.UpdateAsync(customer);
        }

        /// <summary>
        /// Xóa khách hàng có mã là <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">Mã khách hàng</param>
        /// <returns>True nếu xóa thành công</returns>
        public static async Task<bool> DeleteCustomerAsync(int customerId)
        {
            if (await customerDB.IsUsedAsync(customerId))
            {
                return false;
            }
            return await customerDB.DeleteAsync(customerId);
        }

        /// <summary>
        /// Kiểm tra xem khách hàng có dữ liệu liên quan không
        /// </summary>
        /// <param name="customerId">Mã khách hàng</param>
        /// <returns>True nếu đang được sử dụng</returns>
        public static async Task<bool> IsUsedCustomerAsync(int customerId)
        {
            return await customerDB.IsUsedAsync(customerId);
        }

        /// <summary>
        /// Kiểm tra xem email của khách hàng có hợp lệ không
        /// (Email hợp lệ nếu không trùng với khách hàng khác)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="customerId">
        /// Nếu customerId bằng 0 tức là kiểm tra email đối với khách mới.
        /// Nếu khác 0, kiểm tra email của khách hàng có mã là <paramref name="customerId"/>
        /// </param>
        /// <returns></returns>
        public static async Task<bool> ValidateCustomerEmailAsync(string email, int customerId = 0)
        {
            return await customerDB.ValidateEmailAsync(email, customerId);
        }
    }
}
