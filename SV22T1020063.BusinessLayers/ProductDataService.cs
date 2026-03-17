using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.DataLayers.SQLServer;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Catalog;

namespace SV22T1020063.BusinessLayers
{
    /// <summary>
    /// Cung cấp các tính năng xử lý dữ liệu liên quan đến mặt hàng (Product)
    /// </summary>
    public static class ProductDataService
    {
        private static readonly IProductRepository productDB;

        static ProductDataService()
        {
            productDB = new ProductRepository(Configuration.ConnectionString);
        }

        /// <summary>
        /// Tìm kiếm và trả về danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        public static async Task<PagedResult<Product>> ListProductsAsync(ProductSearchInput input)
        {
            return await productDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một mặt hàng dựa theo mã
        /// </summary>
        public static async Task<Product?> GetProductAsync(int productID)
        {
            return await productDB.GetAsync(productID);
        }

        /// <summary>
        /// Bổ sung một mặt hàng mới
        /// </summary>
        public static async Task<int> AddProductAsync(Product data)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            return await productDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một mặt hàng
        /// </summary>
        public static async Task<bool> UpdateProductAsync(Product data)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi cập nhật
            return await productDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một mặt hàng (nếu không có dữ liệu liên quan)
        /// </summary>
        public static async Task<bool> DeleteProductAsync(int productID)
        {
            if (await productDB.IsUsedAsync(productID))
                return false;

            return await productDB.DeleteAsync(productID);
        }

        /// <summary>
        /// Kiểm tra xem mặt hàng hiện có dữ liệu liên quan hay không
        /// </summary>
        public static async Task<bool> IsUsedProductAsync(int productID)
        {
            return await productDB.IsUsedAsync(productID);
        }

        /// <summary>
        /// Lấy danh sách ảnh của một mặt hàng
        /// </summary>
        public static async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            return await productDB.ListPhotosAsync(productID);
        }

        /// <summary>
        /// Lấy thông tin của một ảnh dựa trên mã ảnh
        /// </summary>
        public static async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            return await productDB.GetPhotoAsync(photoID);
        }

        /// <summary>
        /// Bổ sung một ảnh cho mặt hàng
        /// </summary>
        public static async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            return await productDB.AddPhotoAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một ảnh
        /// </summary>
        public static async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            return await productDB.UpdatePhotoAsync(data);
        }

        /// <summary>
        /// Xóa một ảnh của mặt hàng
        /// </summary>
        public static async Task<bool> DeletePhotoAsync(long photoID)
        {
            return await productDB.DeletePhotoAsync(photoID);
        }

        /// <summary>
        /// Lấy danh sách thuộc tính của một mặt hàng
        /// </summary>
        public static async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            return await productDB.ListAttributesAsync(productID);
        }

        /// <summary>
        /// Lấy thông tin một thuộc tính dựa trên mã thuộc tính
        /// </summary>
        public static async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            return await productDB.GetAttributeAsync(attributeID);
        }

        /// <summary>
        /// Bổ sung thuộc tính cho mặt hàng
        /// </summary>
        public static async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            return await productDB.AddAttributeAsync(data);
        }

        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        public static async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            return await productDB.UpdateAttributeAsync(data);
        }

        /// <summary>
        /// Xóa thuộc tính của mặt hàng
        /// </summary>
        public static async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            return await productDB.DeleteAttributeAsync(attributeID);
        }
    }
}
