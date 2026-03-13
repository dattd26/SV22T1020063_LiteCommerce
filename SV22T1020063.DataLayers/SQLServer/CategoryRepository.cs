using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Catalog;

namespace SV22T1020063.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt repository thao tác dữ liệu với bảng Categories trong SQL Server.
    /// Lớp này cung cấp các chức năng CRUD cơ bản và truy vấn danh sách loại hàng
    /// có hỗ trợ tìm kiếm và phân trang.
    /// </summary>
    public class CategoryRepository : IGenericRepository<Category>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo đối tượng CategoryRepository.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến cơ sở dữ liệu SQL Server</param>
        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một loại hàng vào bảng Categories.
        /// </summary>
        /// <param name="data">Thông tin loại hàng cần thêm</param>
        /// <returns>Mã CategoryID của bản ghi vừa được tạo</returns>
        public async Task<int> AddAsync(Category data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Categories(CategoryName, Description)
                           VALUES (@CategoryName, @Description);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<int>(sql, data);
        }

        /// <summary>
        /// Xóa một loại hàng theo CategoryID.
        /// </summary>
        /// <param name="id">Mã loại hàng cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"DELETE FROM Categories WHERE CategoryID = @CategoryID";

            int rows = await connection.ExecuteAsync(sql, new { CategoryID = id });
            return rows > 0;
        }

        /// <summary>
        /// Lấy thông tin một loại hàng theo CategoryID.
        /// </summary>
        /// <param name="id">Mã loại hàng</param>
        /// <returns>Đối tượng Category nếu tồn tại, ngược lại trả về null</returns>
        public async Task<Category?> GetAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT CategoryID, CategoryName, Description
                           FROM Categories
                           WHERE CategoryID = @CategoryID";

            return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { CategoryID = id });
        }

        /// <summary>
        /// Kiểm tra xem loại hàng có đang được sử dụng trong bảng Products hay không.
        /// </summary>
        /// <param name="id">Mã loại hàng</param>
        /// <returns>true nếu đang được tham chiếu</returns>
        public async Task<bool> IsUsedAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT COUNT(*) FROM Products WHERE CategoryID = @CategoryID";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { CategoryID = id });
            return count > 0;
        }

        /// <summary>
        /// Truy vấn danh sách loại hàng có hỗ trợ tìm kiếm và phân trang.
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Kết quả truy vấn dạng phân trang</returns>
        public async Task<PagedResult<Category>> ListAsync(PaginationSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = new PagedResult<Category>()
            {
                Page = input.Page,
                PageSize = input.PageSize
            };

            string searchValue = $"%{input.SearchValue}%";

            string countSql = @"SELECT COUNT(*)
                                FROM Categories
                                WHERE CategoryName LIKE @searchValue";

            result.RowCount = await connection.ExecuteScalarAsync<int>(countSql, new { searchValue });

            if (result.RowCount > 0)
            {
                string querySql;

                if (input.PageSize == 0)
                {
                    querySql = @"SELECT CategoryID, CategoryName, Description
                                 FROM Categories
                                 WHERE CategoryName LIKE @searchValue
                                 ORDER BY CategoryName";
                }
                else
                {
                    querySql = @"SELECT CategoryID, CategoryName, Description
                                 FROM Categories
                                 WHERE CategoryName LIKE @searchValue
                                 ORDER BY CategoryName
                                 OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                }

                var data = await connection.QueryAsync<Category>(querySql, new
                {
                    searchValue,
                    offset = input.Offset,
                    pageSize = input.PageSize
                });

                result.DataItems = data.ToList();
            }

            return result;
        }

        /// <summary>
        /// Cập nhật thông tin một loại hàng.
        /// </summary>
        /// <param name="data">Dữ liệu loại hàng cần cập nhật</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public async Task<bool> UpdateAsync(Category data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Categories
                           SET CategoryName = @CategoryName,
                               Description = @Description
                           WHERE CategoryID = @CategoryID";

            int rows = await connection.ExecuteAsync(sql, data);
            return rows > 0;
        }
    }
}