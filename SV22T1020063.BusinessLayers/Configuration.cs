
namespace SV22T1020063.BusinessLayers
{
    /// <summary>
    /// Lớp này giữ các thông tin cấu hình sử dụng trong Business Layer
    /// </summary>
    public static class Configuration
    {
        private static string _connectionString = "";

        /// <summary>
        /// Hàm có chức năng khởi tạo cấu hình Business Layer
        /// (Hàm này phải được gọi trước khi chạy ứng dụng)
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }
        public static string ConnectionString => _connectionString;
    }
}
