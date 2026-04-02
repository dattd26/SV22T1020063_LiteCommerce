namespace SV22T1020063.Models.Partner
{
    /// <summary>
    /// Khách hàng
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;
        /// <summary>
        /// Tên giao dịch
        /// </summary>
        public string ContactName { get; set; } = string.Empty;
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public string? Province { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Điện thoại
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Khách hàng hiện có bị khóa hay không?
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Trạng thái (dùng cho hiển thị)
        /// </summary>
        public string Status => IsLocked ? "Đã khóa" : "Đang hoạt động";

        /// <summary>
        /// Class CSS cho trạng thái (dùng cho hiển thị)
        /// </summary>
        public string StatusClass => IsLocked ? "badge bg-danger" : "badge bg-success";
    }
}
