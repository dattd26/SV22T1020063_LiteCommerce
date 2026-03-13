using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Partner;

namespace SV22T1020063.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt repository thao tác dữ liệu với bảng Suppliers trong SQL Server.
    /// Lớp này cung cấp các chức năng CRUD cơ bản và truy vấn danh sách nhà cung cấp
    /// có hỗ trợ tìm kiếm và phân trang thông qua Dapper.
    /// </summary>
    public class SupplierRepository : IGenericRepository<Supplier>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo đối tượng SupplierRepository.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến cơ sở dữ liệu SQL Server</param>
        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Bổ sung một nhà cung cấp mới vào bảng Suppliers.
        /// </summary>
        /// <param name="data">Dữ liệu nhà cung cấp cần thêm</param>
        /// <returns>Mã SupplierID của bản ghi vừa được tạo</returns>
        public async Task<int> AddAsync(Supplier data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Suppliers(SupplierName, ContactName, Province, Address, Phone, Email)
                           VALUES (@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<int>(sql, data);
        }

        /// <summary>
        /// Xóa một nhà cung cấp theo SupplierID.
        /// </summary>
        /// <param name="id">Mã nhà cung cấp cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = "DELETE FROM Suppliers WHERE SupplierID = @SupplierID";

            int rows = await connection.ExecuteAsync(sql, new { SupplierID = id });
            return rows > 0;
        }

        /// <summary>
        /// Lấy thông tin một nhà cung cấp theo SupplierID.
        /// </summary>
        /// <param name="id">Mã nhà cung cấp cần truy vấn</param>
        /// <returns>Đối tượng Supplier nếu tồn tại, ngược lại trả về null</returns>
        public async Task<Supplier?> GetAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT SupplierID, SupplierName, ContactName, Province, Address, Phone, Email
                           FROM Suppliers
                           WHERE SupplierID = @SupplierID";

            return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { SupplierID = id });
        }

        /// <summary>
        /// Kiểm tra xem nhà cung cấp có đang được sử dụng bởi bảng Products hay không.
        /// </summary>
        /// <param name="id">Mã nhà cung cấp</param>
        /// <returns>true nếu nhà cung cấp đang được tham chiếu</returns>
        public async Task<bool> IsUsedAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT COUNT(*) FROM Products WHERE SupplierID = @SupplierID";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { SupplierID = id });
            return count > 0;
        }

        /// <summary>
        /// Truy vấn danh sách nhà cung cấp có hỗ trợ tìm kiếm và phân trang.
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Kết quả truy vấn dạng phân trang</returns>
        public async Task<PagedResult<Supplier>> ListAsync(PaginationSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = new PagedResult<Supplier>()
            {
                Page = input.Page,
                PageSize = input.PageSize
            };

            string searchValue = $"%{input.SearchValue}%";

            string countSql = @"SELECT COUNT(*)
                                FROM Suppliers
                                WHERE SupplierName LIKE @searchValue
                                   OR ContactName LIKE @searchValue";

            result.RowCount = await connection.ExecuteScalarAsync<int>(countSql, new { searchValue });

            if (result.RowCount > 0)
            {
                string querySql;

                if (input.PageSize == 0)
                {
                    querySql = @"SELECT SupplierID, SupplierName, ContactName, Province, Address, Phone, Email
                                 FROM Suppliers
                                 WHERE SupplierName LIKE @searchValue
                                    OR ContactName LIKE @searchValue
                                 ORDER BY SupplierName";
                }
                else
                {
                    querySql = @"SELECT SupplierID, SupplierName, ContactName, Province, Address, Phone, Email
                                 FROM Suppliers
                                 WHERE SupplierName LIKE @searchValue
                                    OR ContactName LIKE @searchValue
                                 ORDER BY SupplierName
                                 OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                }

                var data = await connection.QueryAsync<Supplier>(querySql, new
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
        /// Cập nhật thông tin một nhà cung cấp.
        /// </summary>
        /// <param name="data">Dữ liệu nhà cung cấp cần cập nhật</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public async Task<bool> UpdateAsync(Supplier data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Suppliers
                           SET SupplierName = @SupplierName,
                               ContactName = @ContactName,
                               Province = @Province,
                               Address = @Address,
                               Phone = @Phone,
                               Email = @Email
                           WHERE SupplierID = @SupplierID";

            int rows = await connection.ExecuteAsync(sql, data);
            return rows > 0;
        }
    }
}