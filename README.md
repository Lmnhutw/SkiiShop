# SkiiShop

SkiiShop là một ứng dụng bán đồ ski gồm Angular frontend và ASP.NET Core Minimal API backend. Backend dùng Clean Architecture đơn giản với Core, Infrastructure và SShopAPI.

## Trạng thái hiện tại

Đã có:

- Xem danh sách sản phẩm.
- Lọc theo brand/type.
- Sắp xếp theo giá hoặc tên.
- Phân trang.
- Xem chi tiết sản phẩm.
- Thêm sản phẩm vào giỏ, tăng/giảm số lượng và xoá giỏ.
- Admin tạo, sửa, xoá sản phẩm bằng API key.
- Checkout/payment chưa triển khai; nút checkout hiện được giữ ở trạng thái `Coming soon`.

## Kiến trúc thư mục

```text
SkiiShop/
├── Core/                         # Entity và abstraction
├── Infrastructure/               # EF Core, DbContext, repository, seed data
├── SShopAPI/                     # ASP.NET Core Minimal API
│   ├── Endpoints/Product/        # Product endpoints
│   ├── DTOs/                     # Request/response DTOs
│   └── Security/                 # API key authentication
├── client/                       # Angular 20 frontend
│   └── src/app/
│       ├── core/                 # Models, API service, cart, auth interceptor
│       ├── features/catalog/     # Product listing
│       ├── features/product-detail/
│       ├── features/cart/
│       └── features/admin/       # Admin CRUD UI
└── Docker/                       # Docker compose templates
```

## Yêu cầu môi trường

- Git
- .NET SDK 8
- Node.js hỗ trợ Angular 20, khuyến nghị Node.js 20 LTS hoặc mới hơn
- npm
- SQL Server 2022 hoặc SQL Server chạy bằng Docker

Kiểm tra cài đặt:

```powershell
dotnet --version
node --version
npm --version
```

## 1. Clone và cài dependency

```powershell
git clone https://github.com/Lmnhutw/SkiiShop.git
cd SkiiShop

cd client
npm install
cd ..
```

Backend dùng NuGet restore tự động khi build/run. Nếu cần restore thủ công:

```powershell
dotnet restore SkiiShop.sln
```

## 2. Cấu hình SQL Server và secrets

`SShopAPI/appsettings.json` dùng các placeholder sau:

```text
SQL_SERVER_HOST
SQL_SERVER_PORT
SQL_SERVER_DB
SQL_SERVER_USER
SQL_SERVER_PASSWORD
```

Tạo file `.env` ở root repository, cạnh `SkiiShop.sln`:

```dotenv
SQL_SERVER_HOST=localhost
SQL_SERVER_PORT=1433
SQL_SERVER_DB=skiishop
SQL_SERVER_USER=sa
SQL_SERVER_PASSWORD=YourStrongPassword123!
ADMIN_API_KEY=change-this-admin-key
```

Không commit `.env` hoặc API key thật vào Git.

### Chạy SQL Server bằng Docker

Nếu máy đã có SQL Server, chỉ cần dùng các thông tin kết nối tương ứng. Nếu dùng Docker, có thể chạy container SQL Server riêng:

```powershell
docker run --name skiishop-sql `
  -e ACCEPT_EULA=Y `
  -e MSSQL_SA_PASSWORD="YourStrongPassword123!" `
  -p 1433:1433 `
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Sau đó đặt `SQL_SERVER_HOST=localhost`, `SQL_SERVER_PORT=1433`, `SQL_SERVER_USER=sa` và cùng password trong `.env`.

## 3. Chạy backend

Từ root repository:

```powershell
dotnet run --project SShopAPI --launch-profile http
```

API HTTP mặc định:

```text
http://localhost:5210
```

Profile HTTPS dùng:

```powershell
dotnet run --project SShopAPI --launch-profile https
```

API HTTPS mặc định là `https://localhost:7023`.

Khi backend khởi động, ứng dụng sẽ tự chạy EF Core migrations và seed product data. Nếu database chưa sẵn sàng, backend sẽ fail trong bước chuẩn bị database; kiểm tra lại SQL Server và `.env`.

## 4. Chạy frontend

Mở terminal thứ hai:

```powershell
cd client
npm start
```

Mở:

```text
http://localhost:4200
```

Frontend hiện gọi API tại:

```text
http://localhost:5210
```

Cấu hình này nằm ở [api.config.ts](client/src/app/core/config/api.config.ts). Nếu backend chạy ở port khác, cập nhật `API_BASE_URL` và CORS origin tương ứng trong [Program.cs](SShopAPI/Program.cs).

## 5. Các route frontend

| Route | Mục đích |
|---|---|
| `/products` | Danh sách, filter, sort, pagination |
| `/products/:id` | Chi tiết sản phẩm và Add to cart |
| `/cart` | Giỏ hàng local trong browser |
| `/admin/products` | Tạo/sửa/xoá sản phẩm |

Cart hiện lưu trong `localStorage`; chưa có order hoặc payment persistence ở backend.

## 6. API endpoints

### Public endpoints

```http
GET /products
GET /products?id=1
GET /products?brand=BrandName&type=skis&sort=priceasc&page=1&pageSize=12
GET /products/brands
GET /products/types
```

Response danh sách có dạng:

```json
{
  "items": [],
  "totalCount": 0,
  "page": 1,
  "pageSize": 12,
  "totalPages": 0
}
```

### Admin endpoints

Các endpoint sau cần header `X-Admin-Key`:

```http
POST /products/create
PUT /products/updates/{id}
DELETE /products/delete/{id}
```

Ví dụ:

```powershell
$headers = @{ "X-Admin-Key" = "change-this-admin-key" }

Invoke-RestMethod `
  -Method Get `
  -Uri "http://localhost:5210/products" `
  -Headers $headers
```

Admin UI lưu key trong `sessionStorage` của browser và tự gắn header cho request tới `/products`.

## 7. Build và kiểm tra

Build frontend:

```powershell
cd client
npm run build
```

Build toàn bộ solution:

```powershell
cd ..
dotnet build SkiiShop.sln --no-restore
```

Chạy Angular unit tests:

```powershell
cd client
npm test -- --watch=false --browsers=ChromeHeadless
```

Nếu ChromeHeadless không có trong môi trường, cài Chrome hoặc cấu hình một browser launcher phù hợp.

Kiểm tra GitNexus:

```powershell
cd ..
node .gitnexus/run.cjs analyze
```

## Troubleshooting

### `500` hoặc backend dừng khi khởi động

- Kiểm tra SQL Server đang chạy.
- Kiểm tra `.env` nằm ở root repository.
- Kiểm tra password SQL Server đạt policy.
- Kiểm tra database/user/port.

### FE báo không thể tải sản phẩm

- Đảm bảo backend đang chạy ở `http://localhost:5210`.
- Kiểm tra `client/src/app/core/config/api.config.ts`.
- Kiểm tra CORS origin trong `SShopAPI/Program.cs`.
- Mở DevTools để xem status code và response API.

### Admin create/update/delete trả `401` hoặc `403`

- Kiểm tra `ADMIN_API_KEY` trong `.env`.
- Nhập đúng key tại `/admin/products`.
- Kiểm tra header phải là `X-Admin-Key`.
- Khởi động lại backend sau khi đổi `.env`.

### HTTPS báo lỗi certificate local

Trong development có thể dùng profile HTTP để tránh certificate warning:

```powershell
dotnet run --project SShopAPI --launch-profile http
```

## Ghi chú phát triển tiếp theo

- Implement checkout/order/payment API.
- Thêm user authentication thay cho admin API key đơn giản.
- Tách public response DTO khỏi EF entity.
- Thêm integration tests cho product endpoints.
- Thêm e2e test cho catalogue → product detail → cart → checkout.

