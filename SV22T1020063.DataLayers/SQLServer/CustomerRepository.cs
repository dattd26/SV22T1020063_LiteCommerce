using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Partner;

namespace SV22T1020063.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt repository thao tác dữ liệu với bảng Customers trong SQL Server.
    /// Lớp này cung cấp các chức năng CRUD, truy vấn danh sách khách hàng có
    /// hỗ trợ tìm kiếm và phân trang, đồng thời kiểm tra tính hợp lệ của email.
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo đối tượng CustomerRepository.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến cơ sở dữ liệu SQL Server</param>
        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một khách hàng vào bảng Customers.
        /// </summary>
        /// <param name="data">Thông tin khách hàng cần thêm</param>
        /// <returns>Mã CustomerID của bản ghi vừa được tạo</returns>
        public async Task<int> AddAsync(Customer data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                           VALUES (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<int>(sql, data);
        }

        /// <summary>
        /// Xóa một khách hàng theo CustomerID.
        /// </summary>
        /// <param name="id">Mã khách hàng cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"DELETE FROM Customers WHERE CustomerID = @CustomerID";

            int rows = await connection.ExecuteAsync(sql, new { CustomerID = id });
            return rows > 0;
        }

        /// <summary>
        /// Lấy thông tin một khách hàng theo CustomerID.
        /// </summary>
        /// <param name="id">Mã khách hàng</param>
        /// <returns>Đối tượng Customer nếu tồn tại, ngược lại trả về null</returns>
        public async Task<Customer?> GetAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT CustomerID, CustomerName, ContactName, Province,
                                  Address, Phone, Email, IsLocked
                           FROM Customers
                           WHERE CustomerID = @CustomerID";

            return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerID = id });
        }

        /// <summary>
        /// Kiểm tra xem khách hàng có đang được sử dụng trong bảng Orders hay không.
        /// </summary>
        /// <param name="id">Mã khách hàng</param>
        /// <returns>true nếu khách hàng đang được tham chiếu</returns>
        public async Task<bool> IsUsedAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT COUNT(*) FROM Orders WHERE CustomerID = @CustomerID";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { CustomerID = id });
            return count > 0;
        }

        /// <summary>
        /// Truy vấn danh sách khách hàng có hỗ trợ tìm kiếm và phân trang.
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Kết quả truy vấn dạng phân trang</returns>
        public async Task<PagedResult<Customer>> ListAsync(PaginationSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = new PagedResult<Customer>()
            {
                Page = input.Page,
                PageSize = input.PageSize
            };

            string searchValue = $"%{input.SearchValue}%";

            string countSql = @"SELECT COUNT(*)
                                FROM Customers
                                WHERE CustomerName LIKE @searchValue
                                   OR ContactName LIKE @searchValue
                                   OR Phone LIKE @searchValue
                                   OR Email LIKE @searchValue";

            result.RowCount = await connection.ExecuteScalarAsync<int>(countSql, new { searchValue });

            if (result.RowCount > 0)
            {
                string querySql;

                if (input.PageSize == 0)
                {
                    querySql = @"SELECT CustomerID, CustomerName, ContactName, Province,
                                        Address, Phone, Email, IsLocked
                                 FROM Customers
                                 WHERE CustomerName LIKE @searchValue
                                    OR ContactName LIKE @searchValue
                                    OR Phone LIKE @searchValue
                                    OR Email LIKE @searchValue
                                 ORDER BY CustomerName";
                }
                else
                {
                    querySql = @"SELECT CustomerID, CustomerName, ContactName, Province,
                                        Address, Phone, Email, IsLocked
                                 FROM Customers
                                 WHERE CustomerName LIKE @searchValue
                                    OR ContactName LIKE @searchValue
                                    OR Phone LIKE @searchValue
                                    OR Email LIKE @searchValue
                                 ORDER BY CustomerName
                                 OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                }

                var data = await connection.QueryAsync<Customer>(querySql, new
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
        /// Cập nhật thông tin khách hàng.
        /// </summary>
        /// <param name="data">Dữ liệu khách hàng cần cập nhật</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public async Task<bool> UpdateAsync(Customer data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Customers
                           SET CustomerName = @CustomerName,
                               ContactName = @ContactName,
                               Province = @Province,
                               Address = @Address,
                               Phone = @Phone,
                               Email = @Email,
                               IsLocked = @IsLocked
                           WHERE CustomerID = @CustomerID";

            int rows = await connection.ExecuteAsync(sql, data);
            return rows > 0;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của email.
        /// Email hợp lệ nếu chưa tồn tại trong bảng Customers
        /// (hoặc thuộc chính khách hàng đang được cập nhật).
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="id">
        /// Nếu id = 0: kiểm tra email cho khách hàng mới.
        /// Nếu id <> 0: kiểm tra email cho khách hàng đang tồn tại.
        /// </param>
        /// <returns>true nếu email hợp lệ</returns>
        public async Task<bool> ValidateEmailAsync(string email, int id = 0)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql;

            if (id == 0)
            {
                sql = @"SELECT COUNT(*) FROM Customers WHERE Email = @Email";
                int count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
                return count == 0;
            }
            else
            {
                sql = @"SELECT COUNT(*) FROM Customers
                        WHERE Email = @Email AND CustomerID <> @CustomerID";

                int count = await connection.ExecuteScalarAsync<int>(sql, new
                {
                    Email = email,
                    CustomerID = id
                });

                return count == 0;
            }
        }
    }
}