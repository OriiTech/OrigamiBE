# Data Migration Tool - Origami Database

Tool này giúp migrate data từ database local (SQL Server) sang database trên MonsterASP.NET.

## Yêu Cầu

- .NET 8.0 SDK
- SQL Server Local DB với connection string: `Server=.;Database=OrigamiDB;User Id=sa;Password=1;TrustServerCertificate=True;`
- Database trên MonsterASP.NET đã được tạo schema (từ Migration)

## Cách Sử Dụng

### Bước 1: Kiểm tra Connection Strings

Đảm bảo connection strings đúng trong:
- `Program.cs` (dòng 25): Local DB connection string
- `appsettings.json`: Remote DB connection string (MonsterASP.NET)

### Bước 2: Build Project

```bash
cd DataMigration
dotnet build
```

### Bước 3: Chạy Migration

```bash
dotnet run
```

## Lưu Ý

1. **Thứ tự Migration**: Tool tự động migrate theo thứ tự đúng để đảm bảo foreign key constraints được thỏa mãn.

2. **Skip Existing Data**: Nếu bảng đã có data trên remote DB, tool sẽ skip và không ghi đè.

3. **Error Handling**: Nếu có lỗi, tool sẽ dừng và hiển thị thông báo lỗi chi tiết.

4. **EFMigrationHistory**: Table này sẽ KHÔNG được migrate (theo yêu cầu).

## Thứ Tự Migration

Tool migrate theo thứ tự sau để đảm bảo dependencies:

1. Roles, Categories, Badges, TargetLevels, TicketTypes (no dependencies)
2. Users, UserProfiles
3. Origamis, Guides, Courses, Challenges
4. Related tables (Steps, Lessons, Lectures, etc.)
5. User interaction tables (Comments, Favorites, Votes, etc.)
6. Transaction tables (Orders, Transactions, etc.)

## Troubleshooting

### Lỗi Foreign Key Constraint

Nếu gặp lỗi foreign key, có thể do:
- Thứ tự migration chưa đúng
- Data trên local DB không đầy đủ
- Schema trên remote DB khác với local DB

**Giải pháp**: Kiểm tra lại schema và đảm bảo tất cả foreign key tables đã được migrate trước.

### Lỗi Connection

Nếu không kết nối được:
- Kiểm tra connection strings
- Kiểm tra firewall settings
- Kiểm tra SQL Server authentication

### Data Trùng Lặp

Nếu muốn migrate lại (ghi đè data cũ):
- Xóa data trên remote DB trước
- Hoặc modify code để xóa data trước khi insert

## Customization

Nếu cần customize migration:
- Sửa thứ tự migration trong `Program.cs`
- Thêm logic filter data trong method `MigrateTable<T>`
- Thêm logging chi tiết hơn
