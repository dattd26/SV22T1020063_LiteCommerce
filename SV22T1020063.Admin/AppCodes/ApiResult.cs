namespace SV22T1020063.Admin.AppCodes
{
    public class ApiResult
    {
        public ApiResult(int code, string message = "")
        {
            Code = code;
            Message = message;
        }
        /// <summary>
        /// Mã kết quả trả về (qui ước 0 tứ là lỗi hoặc không thành công)
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Thông báo lỗi (nếu có)
        /// </summary>
        public string Message { get; set; } = string.Empty;

    }
}
