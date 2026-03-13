using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Partner;

namespace SV22T1020063.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt repository thao tác dữ liệu với bảng Shippers trong SQL Server.
    /// Cung cấp các chức năng CRUD cơ bản và truy vấn danh sách người giao hàng
    /// có hỗ trợ tìm kiếm và phân trang.
    /// </summary>
    public class ShipperRepository : IGenericRepository<Shipper>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo đối tượng ShipperRepository.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến cơ sở dữ liệu SQL Server</param>
        public ShipperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một người giao hàng vào bảng Shippers.
        /// </summary>
        /// <param name="data">Thông tin người giao hàng cần thêm</param>
        /// <returns>Mã ShipperID của bản ghi vừa được tạo</returns>
        public async Task<int> AddAsync(Shipper data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Shippers(ShipperName, Phone)
                           VALUES (@ShipperName, @Phone);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<int>(sql, data);
        }

        /// <summary>
        /// Xóa một người giao hàng theo ShipperID.
        /// </summary>
        /// <param name="id">Mã người giao hàng cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"DELETE FROM Shippers WHERE ShipperID = @ShipperID";

            int rows = await connection.ExecuteAsync(sql, new { ShipperID = id });
            return rows > 0;
        }

        /// <summary>
        /// Lấy thông tin một người giao hàng theo ShipperID.
        /// </summary>
        /// <param name="id">Mã người giao hàng</param>
        /// <returns>Đối tượng Shipper nếu tồn tại, ngược lại trả về null</returns>
        public async Task<Shipper?> GetAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT ShipperID, ShipperName, Phone
                           FROM Shippers
                           WHERE ShipperID = @ShipperID";

            return await connection.QueryFirstOrDefaultAsync<Shipper>(sql, new { ShipperID = id });
        }

        /// <summary>
        /// Kiểm tra xem người giao hàng có đang được sử dụng trong bảng Orders hay không.
        /// </summary>
        /// <param name="id">Mã người giao hàng</param>
        /// <returns>true nếu đang được tham chiếu</returns>
        public async Task<bool> IsUsedAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT COUNT(*) FROM Orders WHERE ShipperID = @ShipperID";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { ShipperID = id });
            return count > 0;
        }

        /// <summary>
        /// Truy vấn danh sách người giao hàng có hỗ trợ tìm kiếm và phân trang.
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Kết quả truy vấn dạng phân trang</returns>
        public async Task<PagedResult<Shipper>> ListAsync(PaginationSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = new PagedResult<Shipper>()
            {
                Page = input.Page,
                PageSize = input.PageSize
            };

            string searchValue = $"%{input.SearchValue}%";

            string countSql = @"SELECT COUNT(*)
                                FROM Shippers
                                WHERE ShipperName LIKE @searchValue
                                   OR Phone LIKE @searchValue";

            result.RowCount = await connection.ExecuteScalarAsync<int>(countSql, new { searchValue });

            if (result.RowCount > 0)
            {
                string querySql;

                if (input.PageSize == 0)
                {
                    querySql = @"SELECT ShipperID, ShipperName, Phone
                                 FROM Shippers
                                 WHERE ShipperName LIKE @searchValue
                                    OR Phone LIKE @searchValue
                                 ORDER BY ShipperName";
                }
                else
                {
                    querySql = @"SELECT ShipperID, ShipperName, Phone
                                 FROM Shippers
                                 WHERE ShipperName LIKE @searchValue
                                    OR Phone LIKE @searchValue
                                 ORDER BY ShipperName
                                 OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                }

                var data = await connection.QueryAsync<Shipper>(querySql, new
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
        /// Cập nhật thông tin người giao hàng.
        /// </summary>
        /// <param name="data">Dữ liệu cần cập nhật</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public async Task<bool> UpdateAsync(Shipper data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Shippers
                           SET ShipperName = @ShipperName,
                               Phone = @Phone
                           WHERE ShipperID = @ShipperID";

            int rows = await connection.ExecuteAsync(sql, data);
            return rows > 0;
        }
    }
}