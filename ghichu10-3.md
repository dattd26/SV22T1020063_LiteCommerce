## Các đối tượng Supplier, Customer, Shipper, Employee, Category cần các phép xử lý dữ liệu "tương tự" nhau:
	- Tìm kiếm, lấy dữ liệu dưới dạng phân trang
	- Truy vấn dữu liệu của 1 bản ghi (1 dong)
	- Insert được một bản ghi vào bảng
	- Update được một bản ghi vào bảng
	- Delete một bản ghi vào bảng
	- Kiểm tra xem một bản ghi có dữ liệu liên quan hay không? (để ngăn chặn không cho xóa)
	=> Tạo interface định nghĩa các phép xử lỹ dữ liệu cần cài đặt trên các đối tượng này.
	=> interface IGenericRepository<T>
## Với Customer và Employee, Email được sử dụng làm tên đăng nhập => Email không được trùng nhau => Cần có chức năng kiểm tra email có trungf không? => Cần hàm validateEmail => interface trên chưa đủ chức năng cần cài đặt => Thêm interface xử lý dữ liệu cho Employee và cho Customer kế thừa interface trên
	=> IEmployeeRepository : IGenericRepository<Employee>
	=> ICustomerRepository : IGenericRepository<Customer>
## Đối với từ điển dữ liệu:
	- Lấy được DS dữ liệu
	=> interface cho từ điển dữ liệu
	=> interface IdataDictionaryRepository<T>
## Đối với mặt hàng:
	- Tìm kiếm dữ liệu dưới dạng phân trang
		* Đầu vào: Lớp PaginationSearchInput chưa đáp ứng yêu cầu => tạo lớp đầu vào cho tìm kiếm mặt hàng ProductSearchInput kế thừa PaginationSearchInput
	- Lấy được thông tin của một mặt hàng dựa vào id
	- Bổ sung 
	- Cập nhật
	- Xóa
	- Mặt hàng đã có dữ liệu liên quan chưa?
	- Lấy danh sách thuộc tính
	- Bổ sung thuộc tính
	- Cập nhật thuộc tính
	- Xóa thuộc tính
	- Lấy danh sách ảnh của mặt hàng
	- Lấy thông tin 1 ảnh
	- Cập nhật ảnh 
	- Xóa ảnh 
	=> interface IProductRepository  
## Đối với đơn hàng:
	- Tìm kiếm, hiển thị phân trang
		* Đầu vào: OrdeSearchInput kế thừa PaginationSearchInput  
		* Đầu ra: OrderViewInfo (lớp DTO) để biểu diễn thông tin của đơn hàng (không sử dụng lớp Order)
	- Xem thông tin đầy đủ của một đơn hàng
	- Bổ sung đơn hàng
	- Cập nhật đơn hàng
	- Xóa đơn hàng
	- Lấy danh sách mặt hàng bán trong đơn hàng
		* Sử dụng DTO OrderDetailViewInfo để biểu diễn dữ liệu (không sử dụng OrderDetail)
	- Lấy thông tin 1 mặt hàn bán trong đơn hàng
	- Cập nhật một mặt hàng trong đơn hàng (số lượng, giá bán)
	- Xóa mặt hàng khởi đơn hàng
	=> interface IOrderRepository
## Liên quan đến tài khoản (của Employee, của Customer)
	- Kiểm tra thông tin đăng nhập có hợp lệ?
	- Đổi mật khẩu
	=> interface IUserAccountRepository 



	khong update view thi redirec action