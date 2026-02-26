# ⚽ Football Management System - WPF

> **Môn:** Lập trình ứng dụng .NET  
> **Trường:** Đại học Duy Tân  
> **Ngày:** 26/02/2026  
> **Nhóm 3:** 10 thành viên  
> **Đề tài:** Quản lý hệ thống giải đấu bóng đá  

---

## 🎯 Mục tiêu dự án

Xây dựng ứng dụng **WPF** phục vụ quản lý một **giải đấu bóng đá** (đội tham gia, cầu thủ, lịch thi đấu, kết quả, bảng xếp hạng, thống kê), dữ liệu lưu trữ bằng **SQL Server**.

---

## 📂 Cấu trúc dự án

```
Football_Management_System/
├── App.xaml / App.xaml.cs                  ← Khởi động ứng dụng
├── DataAccess/
│   └── DatabaseHelper.cs                  ← Kết nối & thao tác DB (Đức)
├── Database/
│   ├── FootballDB_Master.sql              ← ⭐ DB chuẩn duy nhất (Single Source of Truth)
│   ├── FootballDB_Schema.sql              ← DB riêng phần Kết quả trận đấu
│   └── FootballDB_All.sql                 ← DB tổng hợp (bản cũ, tham khảo)
├── Properties/
│   └── AssemblyInfo.cs
│
├── LoginWindow.xaml/.cs                   ← Đăng nhập & phân quyền (Quân)
├── QuanLyGiaiDau.xaml/.cs                 ← Quản lý giải đấu (Chinh)
├── QuanLyDoiBong.xaml/.cs                 ← Quản lý đội bóng (Bảo)
├── PlayerManagement.xaml/.cs              ← Quản lý cầu thủ (Việt)
├── CoachManagementWindow.xaml/.cs         ← Quản lý huấn luyện viên (Đạt)
├── LichThiDau.xaml/.cs                    ← Lịch thi đấu (Hiền)
├── MatchResultMainWindow.xaml/.cs         ← Màn hình chính Kết quả (Đức)
├── MatchResultWindow.xaml/.cs             ← Quản lý kết quả trận đấu (Đức)
├── StandingsWindow.xaml/.cs               ← Bảng xếp hạng (Độ)
├── StatisticsWindow.xaml/.cs              ← Thống kê & giải thưởng (Phúc)
├── ReportWindow.xaml/.cs                  ← Báo cáo & xuất dữ liệu (Huy)
└── Football_Management_System.csproj
```

---

## 👥 Phân công nhiệm vụ chi tiết

| # | Thành viên | Window | Chức năng chính |
|---|------------|--------|-----------------|
| 1 | **Quân** | `LoginWindow` | Đăng nhập hệ thống, phân quyền Admin / Người quản lý, kiểm tra hợp lệ tài khoản |
| 2 | **Chinh** | `QuanLyGiaiDau` | Thêm / sửa / xóa giải đấu, thiết lập số vòng đấu, thời gian bắt đầu – kết thúc |
| 3 | **Bảo** | `QuanLyDoiBong` | Thêm / sửa / xóa đội bóng, thông tin đội (tên, logo, HLV), gán đội vào giải đấu |
| 4 | **Việt** | `PlayerManagement` | Thêm / sửa / xóa cầu thủ, số áo, vị trí, ngày sinh, gán cầu thủ cho đội |
| 5 | **Đạt** | `CoachManagementWindow` | Thông tin HLV, gán HLV cho đội bóng, theo dõi lịch sử dẫn dắt |
| 6 | **Hiền** | `LichThiDau` | Tạo lịch thi đấu tự động, chỉnh sửa lịch, hiển thị lịch theo vòng / đội |
| 7 | **Đức** | `MatchResultWindow` | Nhập kết quả trận, bàn thắng, thẻ phạt, cập nhật kết quả vào hệ thống |
| 8 | **Độ** | `StandingsWindow` | Tự động tính điểm, sắp xếp theo điểm / hiệu số, cập nhật theo thời gian thực |
| 9 | **Phúc** | `StatisticsWindow` | Vua phá lưới, cầu thủ kiến tạo, thống kê số bàn thắng / thẻ |
| 10 | **Huy** | `ReportWindow` | Xuất báo cáo PDF / Excel, thống kê tổng quan giải đấu, hỗ trợ in ấn |

---

## 📊 Trạng thái trên nhánh `main` (GitHub)

| # | Thành viên | Nhánh Git | File XAML trên main | Trạng thái |
|---|------------|-----------|---------------------|------------|
| 1 | **Quân** | `MinhQuan` | `LoginWindow.xaml/.cs` | ✅ Đã push *(nằm trong subfolder sai)* |
| 2 | **Chinh** | `chinh` | `QuanLyGiaiDau.xaml/.cs` | ✅ Đã push |
| 3 | **Bảo** | `bao` | `QuanLyDoiBong.xaml/.cs` | ✅ Đã push |
| 4 | **Việt** | `Viet` | `FormQuanLyCauThu.xaml/.cs` | ✅ Đã push *(nằm trong subfolder sai)* |
| 5 | **Đạt** | `Dat` | `QuanLyHuanLuyenVien.xaml/.cs` | ✅ Đã push |
| 6 | **Hiền** | `Hien` | `LichThiDau.xaml/.cs` | ✅ Đã push *(nằm trong subfolder sai)* |
| 7 | **Đức** | `duc` | `MatchResultMainWindow` + `MatchResultWindow` | ✅ Đã push + code-behind + DB |
| 8 | **Độ** | `QuocDo` | `StandingsWindow.xaml/.cs` | ✅ Đã push *(nằm trong subfolder sai)* |
| 9 | **Phúc** | `TranPhuc` | `StatisticsAndAwardsWindow.xaml/.cs` + Models + ViewModels | ✅ Đã push *(nằm trong subfolder sai)* |
| 10 | **Huy** | `Huy` | ❌ **Không có `ReportWindow` trên main** | ❌ **Chưa push lên main** |

### Tổng kết

| Trạng thái | Số người | Ai |
|------------|----------|-----|
| ✅ Đã push lên main | **9** | Quân, Chinh, Bảo, Việt, Đạt, Hiền, Đức, Độ, Phúc |
| ❌ Chưa push lên main | **1** | **Huy** (ReportWindow chỉ có trên nhánh `Huy` và `test`) |

### ⚠️ Lưu ý: File đặt sai thư mục trên main

Một số thành viên push file vào thư mục con `Football_Management_System/Football_Management_System/` thay vì `Football_Management_System/`:

| Thành viên | Vị trí sai | Cần di chuyển về |
|------------|-----------|-----------------|
| Quân | `.../Football_Management_System/LoginWindow.xaml` | `Football_Management_System/LoginWindow.xaml` |
| Việt | `.../Football_Management_System/FormQuanLyCauThu.xaml` | `Football_Management_System/PlayerManagement.xaml` |
| Hiền | `.../Football_Management_System/LichThiDau.xaml` | `Football_Management_System/LichThiDau.xaml` |
| Độ | `.../Football_Management_System/StandingsWindow.xaml` | `Football_Management_System/StandingsWindow.xaml` |
| Phúc | `.../Football_Management_System/Statistics/Views/...` | `Football_Management_System/StatisticsWindow.xaml` |

---

## 🗄️ Database

### ⭐ File DB chuẩn: `FootballDB_Master.sql`

Database chuẩn duy nhất cho cả nhóm — **Single Source of Truth**. Gồm 16 bảng, 1 view, 12 stored procedures, dữ liệu mẫu.

| Cấp | Bảng | Phụ trách | Mô tả |
|-----|------|-----------|-------|
| 1 (Nền tảng) | `Tournaments` | Chinh | Giải đấu |
| 1 | `Coaches` | Đạt | Huấn luyện viên |
| 1 | `Roles` | Quân | Phân quyền |
| 2 | `Teams` | Bảo | Đội bóng *(bảng chuẩn duy nhất)* |
| 2 | `Users` | Quân | Người dùng |
| 2 | `Rounds` | Độ | Vòng đấu |
| 3 | `Players` | Việt | Cầu thủ |
| 3 | `Matches` | Đức / Hiền | Trận đấu |
| 3 | `CoachHistory` | Đạt | Lịch sử HLV |
| 4 | `MatchResults` | Đức | Kết quả trận đấu |
| 4 | `Standings` | Độ | Bảng xếp hạng |
| 4 | `PlayerStatistics` | Độ | Thống kê cầu thủ theo trận |
| 4 | `PlayerGeneralStatistics` | Việt | Thống kê tổng cầu thủ |
| 4 | `PlayerNotes` | Việt | Ghi chú cầu thủ |
| 4 | `PlayerAttachments` | Việt | Đính kèm cầu thủ |

**Quan hệ chính:**
```
[Tournaments] 1 ── n [Teams] n ── 1 [Coaches]
                       │
              ┌────────┼────────┐
              ▼        ▼        ▼
          [Players] [Matches] [CoachHistory]
                       │
                       ▼
                 [MatchResults]
                 [Standings]
```

---

## 🚀 Hướng dẫn chạy

### Bước 1: Clone & checkout nhánh test
```bash
git clone https://github.com/Duc140405/FootBallCSharp.git
cd FootBallCSharp
git checkout test
```

### Bước 2: Tạo database
Mở **SQL Server Management Studio** hoặc **sqlcmd** và chạy file **`FootballDB_Master.sql`**:
```sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "Football_Management_System\Database\FootballDB_Master.sql"
```

### Bước 3: Kiểm tra connection string
Connection string được lưu trong `App.config` (KHÔNG hardcode trong code):
```xml
<connectionStrings>
    <add name="FootballDB" 
         connectionString="Server=(localdb)\MSSQLLocalDB;Database=FootballManagementDB;Integrated Security=True;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```
> **Nếu dùng SQL Server Express:** Mở `App.config`, đổi `Server=(localdb)\MSSQLLocalDB` thành `Server=.\SQLEXPRESS`. Không sửa file C#.

### Bước 4: Build & Run
Mở `Football_Management_System.sln` bằng Visual Studio → Build → Run (F5).

---

## ✅ Những gì đã hoàn thành (Tích hợp nhánh `test`)

- [x] **FootballDB_Master.sql** — DB chuẩn duy nhất (16 bảng + 1 view + 12 SPs + sample data)
- [x] **App.config** — Connection string tập trung, không hardcode
- [x] **DatabaseHelper.cs** — Dùng `ConfigurationManager` đọc từ `App.config`
- [x] **Namespace** — Đã thống nhất tất cả thành `Football_Management_System`
- [x] **CoachManagementWindow** — Sửa `x:Class` từ `Window1` → `CoachManagementWindow`
- [x] **LoginWindow** — Đổi namespace từ `Views` về root
- [x] **App.xaml** — `StartupUri` đổi sang `LoginWindow.xaml`
- [x] **Build** — ✅ Thành công

---

## 📋 TODO cho các thành viên

- [ ] **Huy:** Push `ReportWindow` lên nhánh `main`, cài NuGet (`ClosedXML`, `iTextSharp`)
- [ ] **Quân, Việt, Hiền, Độ, Phúc:** Di chuyển file XAML từ subfolder sai về đúng thư mục gốc project
- [ ] **Tất cả:** Pull nhánh `test`, chạy `FootballDB_Master.sql` trên SQL Server
- [ ] **Tất cả:** Dùng `new DatabaseHelper().GetConnection()` để kết nối DB, **KHÔNG** tự viết connection string
- [ ] **Tất cả:** Namespace phải là `Football_Management_System` (đã sửa trên nhánh `test`)
