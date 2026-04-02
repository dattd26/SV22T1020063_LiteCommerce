


## Nguyên tắc chung:
- Người dùng sử dụng cung cấp thông tin để kiểm tra (username + password/sinh trắc học)
- **Hệ thống kiểm tra xem thông tin có hợp lệ không? Nếu hợp lệ cấp cho người dùng một "chứng nhận"(principal) (Authentication)**  và giao cho người dùng (cookie) access token
- Phía client lưu giữ cookie, và đính kièm cookie (trong phần header) mỗi khi có request lên server.
- **Phía server dựa vào cookie để kiểm tra xem người dùng có hợp lệ không (Authorization)**:
	- Chuẩn bị thông tin ghi lên giấy chứng nhận
	- Tạo ra giấy chứng nhận
	- Giao chứng nhận cho người dùng (cookie)
## Thuật ngữ:
- Authentication
- Authorization
## Trong ASP.NET Core, muốn sử dụng Auth thì phải đăng ký.

- Để sử dụng cơ chế Authorization đối với các Controller hoặc Action, dặt chỉ thị sau phía trước:
	[Authorize]
- Để bỏ Authorize:
	[AllowAnonymous]
- Trong action hoặc View, thông qua thuộc tính User để lấy được "Giấy chứng nhận" đã cấp cho client

## Trong trường hợp cần ktra quyền của người dùng (đã login), sử dụng chỉ thị:
- [Authorize(Roles = "Danh_sách_quyền")]