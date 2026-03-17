using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.DataLayers.SQLServer;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.HR;

namespace SV22T1020063.BusinessLayers
{
    /// <summary>
    /// Cung cấp các tính năng xử lý dữ liệu liên quan đến nhân viên (Human Resource)
    /// </summary>
    public static class HRDataService
    {
        private static readonly IEmployeeRepository employeeDB;

        static HRDataService()
        {
            employeeDB = new EmployeeRepository(Configuration.ConnectionString);
        }

        /// <summary>
        /// Tìm kiếm và trả về danh sách nhân viên dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách nhân viên dạng phân trang</returns>
        public static async Task<PagedResult<Employee>> ListEmployeeAsync(PaginationSearchInput input)
        {
            return await employeeDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một nhân viên dựa vào mã
        /// </summary>
        /// <param name="id">Mã nhân viên cần lấy thông tin</param>
        /// <returns>Thông tin nhân viên nếu tồn tại, ngược lại trả về null</returns>
        public static async Task<Employee?> GetEmployeeAsync(int id)
        {
            return await employeeDB.GetAsync(id);
        }

        /// <summary>
        /// Bổ sung một nhân viên mới
        /// </summary>
        /// <param name="data">Thông tin nhân viên cần bổ sung</param>
        /// <returns>Mã nhân viên được tạo</returns>
        public static async Task<int> AddEmployeeAsync(Employee data)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            return await employeeDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhân viên
        /// </summary>
        /// <param name="data">Thông tin nhân viên cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, ngược lại False</returns>
        public static async Task<bool> UpdateEmployeeAsync(Employee data)
        {
            // TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi cập nhật
            return await employeeDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một nhân viên (nếu không có dữ liệu liên quan)
        /// </summary>
        /// <param name="id">Mã nhân viên cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu không thể xóa</returns>
        public static async Task<bool> DeleteEmployeeAsync(int id)
        {
            if (await employeeDB.IsUsedAsync(id))
                return false;

            return await employeeDB.DeleteAsync(id);
        }

        /// <summary>
        /// Kiểm tra xem nhân viên hiện đang có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id">Mã nhân viên</param>
        /// <returns>True nếu đang được sử dụng, False nếu không</returns>
        public static async Task<bool> IsUsedEmployeeAsync(int id)
        {
            return await employeeDB.IsUsedAsync(id);
        }

        /// <summary>
        /// Kiểm tra xem email có hợp lệ hay không (không trùng với email của người khác)
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="id">
        /// Nếu id bằng 0 tức là kiểm tra email đối với nhân viên mới.
        /// Nếu khác 0, kiểm tra email của nhân viên có mã là id.
        /// </param>
        /// <returns>True nếu email hợp lệ, ngược lại False</returns>
        public static async Task<bool> ValidateEmployeeEmailAsync(string email, int id = 0)
        {
            return await employeeDB.ValidateEmailAsync(email, id);
        }
    }
}
