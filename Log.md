
Tạo blank solution có tên SV<MaSV> 
Bổ sung cho solution các projects:
- <SolutionName>.Admin: project dạng ASP.NET Core MVC 
- <SolutionName>.Shop: project dạng ASP.NET Core MVC 
- <SolutionName>.Models: Class library
- <SolutionName>.DataLayers: Class library
- <SolutionName>.BusinessLayers: Class library

# Thiết kế Layout cho ứng dụng Admin

- Sử dụng AdminLTE4, Bootstrap5
- Thay đổi file _Layout.cshtml
- 
# Các Controller và Action dự kiến (chức năng dự kiến)

## Home
- Home/Index

## Account
- Account/Login
- Account/Logout
- Account/ChangePassword

## Supplier
- Supplier/Index
	- Hiển thị danh sách nhà cung cấp dưới dạng phân trang
	- Tìm kiếm nhà cung cấp
	- Điều hướng (navigate/link) đến các chức năng khác liên quan đến nhà cung cấp
- Supplier/Create
- Supplier/Edit/{id}
- Supplier/Delete/{id}

## Customer
- Customer/Index
- Customer/Create
- Customer/Edit/{id}
- Customer/Delete/{id}
- Customer/ChangePassword/{id}

## Shipper
- Shipper/Index
- Shipper/Create
- Shipper/Edit/{id}
- Shipper/Delete/{id}
-
## Employee
- Employee/Index
<<<<<<< HEAD
=======
- 
>>>>>>> 47b8d31b5cba9f97d73b91bc1c666abcd974b432
- Employee/Create
- Employee/Edit/{id}
- Employee/Delete/{id}
- Employee/ChangePassword/{id}
- Employee/ChangeRole/{id}

## Category
- Category/Index
- Category/Create
- Category/Edit/{id}
- Category/Delete/{id}

## Product
- Product/Index
<<<<<<< HEAD
	- Hiển thị danh sách mặt hàng dưới dạng phân trangx
=======
	- Hiển thị danh sách mặt hàng dưới dạng phân trang
>>>>>>> 47b8d31b5cba9f97d73b91bc1c666abcd974b432
	- Tìm kiếm/lọc mặt hàng: Loại hàng (select), nhà cung cấp (select), khoảng giá,...
- Product/Detail/{id}
- Product/Create
- Product/Edit/{id}
- Product/Delete/{id}
- Product/ListAttributes/{id}
- Product/CreateAttribute/{id}
- Product/EditAttribute/{id}?attributeId={attributeId}
- Product/DeleteAttribute/{id}?attributeId={attributeId}
- Product/ListPhoto/{id}
- Product/CreatePhoto/{id}
- Product/EditPhoto/{id}?photoId={photoId}
- Product/DeletePhoto/{id}?photoId={photoId}

## Order
- Order/Index
- Order/Create
- Order/Edit/{id}
- Order/Delete/{id}

