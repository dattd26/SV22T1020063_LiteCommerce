# LiteCommerce - SV22T1020063

Dự án LiteCommerce là một ứng dụng thương mại điện tử được xây dựng bằng ASP.NET Core MVC , bao gồm hai cổng thông tin chính: **Admin** (Quản trị) và **Shop** (Cửa hàng cho khách hàng).

Hệ thống được thiết kế theo mô hình kiến trúc 3 Layers (3-tier architecture) nhằm đảm bảo tính tách biệt, dễ bảo trì và mở rộng
1. Presentation Layer (Tầng trình diễn)
Đây là tầng giao diện người dùng, chịu trách nhiệm hiển thị dữ liệu và tiếp nhận tương tác từ người dùng.

2. Business Logic Layer (Tầng xử lý nghiệp vụ)
Tầng này chứa toàn bộ logic nghiệp vụ của hệ thống.
Xử lý các quy tắc như: tính giá, kiểm tra tồn kho, xử lý đơn hàng,...

3. Data Access Layer (Tầng truy xuất dữ liệu)
Tầng này chịu trách nhiệm làm việc trực tiếp với cơ sở dữ liệu. Thực hiện các thao tác CRUD (Create, Read, Update, Delete)

## 🚀 Tính năng chính
- Quản lý danh mục, sản phẩm, khách hàng, nhân viên và đối tác.
- Hệ thống đặt hàng và quản lý đơn hàng.
- Giao diện Admin chuyên nghiệp (AdminLTE).
- Giao diện Shop hiện đại sử dụng **Tailwind CSS** và plugin **Flowbite**.

## 🛠️ Yêu cầu hệ thống
Trước khi bắt đầu, hãy đảm bảo máy tính của bạn đã cài đặt các công cụ sau:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) hoặc mới hơn.
- [Node.js](https://nodejs.org/) (để quản lý các gói frontend cho phần Shop).
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express hoặc Developer edition).

## 📂 Hướng dẫn cài đặt

### 1. Clone repository
Mở terminal và chạy lệnh:
```bash
git clone https://github.com/dattd26/SV22T1020063_LiteCommerce.git
cd SV22T1020063_LiteCommerce
```

### 2. Thiết lập cơ sở dữ liệu
1. Mở **SQL Server Management Studio (SSMS)**.
2. Tạo một cơ bả dữ liệu mới tên là `LiteCommerceDB`.
3. Mở file `LiteCommerceDB_Update2026.sql` (ở thư mục gốc của dự án) và chạy script này trên DB vừa tạo.
4. Kiểm tra chuỗi kết nối trong file `appsettings.json` của cả 2 dự án `SV22T1020063.Admin` và `SV22T1020063.Shop`:
   ```json
   "ConnectionStrings": {
     "LiteCommerceDB": "Server=.;Database=LiteCommerceDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
   *(Lưu ý: Thay đổi `Server=.` nếu SQL Server của bạn có instance name khác).*

### 3. Cài đặt các gói NPM (Chỉ dành cho Shop)
Phần giao diện Shop sử dụng Tailwind CSS và Flowbite. Bạn cần cài đặt các dependency này:
```bash
cd SV22T1020063.Shop
npm install
```

Để build lại CSS (khi bạn thay đổi class Tailwind):
```bash
npm run build:css
```

## 🏗️ Build và Chạy ứng dụng

### Chạy bằng Terminal
Ở thư mục dự án tương ứng, chạy lệnh:
```bash
# Để chạy cổng Admin
cd SV22T1020063.Admin
dotnet run

# Để chạy cổng Shop
cd SV22T1020063.Shop
dotnet run
```

### Chạy bằng Visual Studio
1. Mở file `SV22T1020063.sln` bằng Visual Studio.
2. Chuột phải vào dự án (`Admin` hoặc `Shop`) và chọn **Set as Startup Project**.
3. Nhấn **F5** hoặc nút **Start**.

## 📝 Cấu trúc dự án
- `SV22T1020063.Admin`: Dự án Web quản trị.
- `SV22T1020063.Shop`: Dự án Web bán hàng.
- `SV22T1020063.BusinessLayers`: Chứa logic nghiệp vụ.
- `SV22T1020063.DataLayers`: Chứa các lớp tương tác dữ liệu (Dapper/ADO.NET).
- `SV22T1020063.Models`: Chứa các lớp đối tượng (POCO).

---
*Phát triển bởi: Trần Đức Đạt*