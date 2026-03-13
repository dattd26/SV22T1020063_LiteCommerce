using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.HR;

namespace SV22T1020063.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt repository thao tác dữ liệu với bảng Employees trong SQL Server.
    /// Lớp này cung cấp các chức năng CRUD, truy vấn danh sách nhân viên có
    /// hỗ trợ tìm kiếm và phân trang, đồng thời kiểm tra tính hợp lệ của email.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo EmployeeRepository.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối SQL Server</param>
        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một nhân viên.
        /// </summary>
        public async Task<int> AddAsync(Employee data)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"INSERT INTO Employees(FullName, BirthDate, Address, Phone, Email, Photo, IsWorking)
                           VALUES(@FullName,@BirthDate,@Address,@Phone,@Email,@Photo,@IsWorking);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(sql, data);
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên.
        /// </summary>
        public async Task<bool> UpdateAsync(Employee data)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"UPDATE Employees
                           SET FullName=@FullName,
                               BirthDate=@BirthDate,
                               Address=@Address,
                               Phone=@Phone,
                               Email=@Email,
                               Photo=@Photo,
                               IsWorking=@IsWorking
                           WHERE EmployeeID=@EmployeeID";

            return await connection.ExecuteAsync(sql, data) > 0;
        }

        /// <summary>
        /// Xóa nhân viên theo mã.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"DELETE FROM Employees
                           WHERE EmployeeID=@EmployeeID";

            return await connection.ExecuteAsync(sql, new { EmployeeID = id }) > 0;
        }

        /// <summary>
        /// Lấy thông tin một nhân viên theo mã.
        /// </summary>
        public async Task<Employee?> GetAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"SELECT EmployeeID, FullName, BirthDate, Address,
                                  Phone, Email, Photo, IsWorking
                           FROM Employees
                           WHERE EmployeeID=@EmployeeID";

            return await connection.QueryFirstOrDefaultAsync<Employee>(
                sql,
                new { EmployeeID = id }
            );
        }

        /// <summary>
        /// Kiểm tra nhân viên có dữ liệu liên quan trong bảng Orders hay không.
        /// </summary>
        public async Task<bool> IsUsedAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"SELECT COUNT(*)
                           FROM Orders
                           WHERE EmployeeID=@EmployeeID";

            int count = await connection.ExecuteScalarAsync<int>(
                sql,
                new { EmployeeID = id }
            );

            return count > 0;
        }

        /// <summary>
        /// Truy vấn danh sách nhân viên có hỗ trợ tìm kiếm và phân trang.
        /// </summary>
        public async Task<PagedResult<Employee>> ListAsync(PaginationSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = new PagedResult<Employee>()
            {
                Page = input.Page,
                PageSize = input.PageSize
            };

            string searchValue = $"%{input.SearchValue}%";

            string countSql = @"SELECT COUNT(*)
                                FROM Employees
                                WHERE FullName LIKE @searchValue
                                   OR Phone LIKE @searchValue
                                   OR Email LIKE @searchValue";

            result.RowCount = await connection.ExecuteScalarAsync<int>(
                countSql,
                new { searchValue }
            );

            if (result.RowCount > 0)
            {
                string sql;

                if (input.PageSize == 0)
                {
                    sql = @"SELECT EmployeeID, FullName, BirthDate, Address,
                                   Phone, Email, Photo, IsWorking
                            FROM Employees
                            WHERE FullName LIKE @searchValue
                               OR Phone LIKE @searchValue
                               OR Email LIKE @searchValue
                            ORDER BY FullName";
                }
                else
                {
                    sql = @"SELECT EmployeeID, FullName, BirthDate, Address,
                                   Phone, Email, Photo, IsWorking
                            FROM Employees
                            WHERE FullName LIKE @searchValue
                               OR Phone LIKE @searchValue
                               OR Email LIKE @searchValue
                            ORDER BY FullName
                            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                }

                var data = await connection.QueryAsync<Employee>(sql, new
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
        /// Kiểm tra tính hợp lệ của email nhân viên.
        /// Email hợp lệ nếu chưa tồn tại trong bảng Employees
        /// (hoặc thuộc chính nhân viên đang cập nhật).
        /// </summary>
        public async Task<bool> ValidateEmailAsync(string email, int id = 0)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql;

            if (id == 0)
            {
                sql = @"SELECT COUNT(*) FROM Employees WHERE Email=@Email";
                int count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
                return count == 0;
            }
            else
            {
                sql = @"SELECT COUNT(*) FROM Employees
                        WHERE Email=@Email AND EmployeeID<>@EmployeeID";

                int count = await connection.ExecuteScalarAsync<int>(sql, new
                {
                    Email = email,
                    EmployeeID = id
                });

                return count == 0;
            }
        }
    }
}
