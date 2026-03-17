using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.DataLayers.SQLServer;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Catalog;

namespace SV22T1020063.BusinessLayers
{
    /// <summary>
    /// Cung cấp các tính năng xử lý dữ liệu liên quan đến loại hàng (Category)
    /// </summary>
    public static class CategoryDataService
    {
        private static readonly IGenericRepository<Category> categoryDB;

        static CategoryDataService()
        {
            categoryDB = new CategoryRepository(Configuration.ConnectionString);
        }

        /// <summary>
        /// Tìm kiếm và trả về danh sách loại hàng dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách loại hàng dạng phân trang</returns>
        public static async Task<PagedResult<Category>> ListCategoryAsync(PaginationSearchInput input)
        {
            return await categoryDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một loại hàng dựa trên mã
        /// </summary>
        /// <param name="id">Mã loại hàng cần lấy thông tin</param>
        /// <returns>Thông tin loại hàng nếu tồn tại, ngược lại trả về null</returns>
        public static async Task<Category?> GetCategoryAsync(int id)
        {
            return await categoryDB.GetAsync(id);
        }

        /// <summary>
        /// Bổ sung một loại hàng mới
        /// </summary>
        /// <param name="data">Thông tin loại hàng cần bổ sung</param>
        /// <returns>Mã loại hàng được tạo</returns>
        public static async Task<int> AddCategoryAsync(Category data)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            return await categoryDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một loại hàng
        /// </summary>
        /// <param name="data">Thông tin loại hàng cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, ngược lại False</returns>
        public static async Task<bool> UpdateCategoryAsync(Category data)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi cập nhật
            return await categoryDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một loại hàng (nếu không có dữ liệu liên quan)
        /// </summary>
        /// <param name="id">Mã loại hàng cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu không thể xóa</returns>
        public static async Task<bool> DeleteCategoryAsync(int id)
        {
            if (await categoryDB.IsUsedAsync(id))
                return false;

            return await categoryDB.DeleteAsync(id);
        }

        /// <summary>
        /// Kiểm tra xem loại hàng hiện đang có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id">Mã loại hàng</param>
        /// <returns>True nếu đang được sử dụng, False nếu không</returns>
        public static async Task<bool> IsUsedCategoryAsync(int id)
        {
            return await categoryDB.IsUsedAsync(id);
        }
    }
}
