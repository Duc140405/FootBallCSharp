# ⚽ Football Management System - WPF

> **Môn:** Lập trình ứng dụng .NET  
> **Trường:** Đại học Duy Tân  
> **Ngày:** 26/02/2026  
> **Nhóm:** 10 thành viên  

---

## 📂 Cấu trúc dự án

```
Football_Management_System/
├── App.xaml / App.xaml.cs              ← Khởi động ứng dụng
├── DataAccess/
│   └── DatabaseHelper.cs              ← Kết nối & thao tác DB (Đức)
├── Database/
│   ├── FootballDB_Schema.sql          ← DB riêng từng thành viên
│   └── FootballDB_All.sql             ← DB tổng hợp tất cả thành viên
├── Properties/
│   └── AssemblyInfo.cs
│
├── MatchResultMainWindow.xaml/.cs     ← Màn hình chính Match Result (Đức)
├── MatchResultWindow.xaml/.cs         ← Quản lý kết quả trận đấu (Đức)
├── LoginWindow.xaml/.cs               ← Đăng nhập
├── PlayerManagement.xaml/.cs          ← Quản lý cầu thủ
├── CoachManagementWindow.xaml/.cs     ← Quản lý huấn luyện viên
├── QuanLyDoiBong.xaml/.cs             ← Quản lý đội bóng
├── StandingsWindow.xaml/.cs           ← Bảng xếp hạng
├── ReportWindow.xaml/.cs              ← Báo cáo / Xuất file
├── StatisticsWindow.xaml/.cs          ← Thống kê
└── Football_Management_System.csproj
```

---

## 👥 Phân công thành viên & trạng thái

| # | Thành viên | Nhánh Git | XAML đã push | Chức năng | DB trong FootballDB_All | Trạng thái |
|---|------------|-----------|-------------|-----------|------------------------|------------|
| 1 | **Nguyễn Tấn Đức** | `duc` | `MatchResultMainWindow` + `MatchResultWindow` | Quản lý kết quả trận đấu | `Teams`, `Matches`, `MatchResults` + View + 5 SPs + data mẫu | ✅ **Hoàn thành** |
| 2 | **QuangBao** | `QuangBao` | `LoginWindow` | Đăng nhập / Phân quyền | `Roles`, `Users` | ✅ Đã push XAML + code-behind |
| 3 | **Ngọc Việt** | `Viet` | `PlayerManagement` | Quản lý cầu thủ | `players`, `player_general_statistics`, `player_notes`, `player_attachments` | ⚠️ Chỉ có XAML UI, **chưa có code-behind logic + chưa có DB riêng** |
| 4 | **TranPhuc** | `TranPhuc` | `StatisticsWindow` + `CoachManagementWindow` | Thống kê + HLV | `coaches`, `coach_history` | ⚠️ `CoachManagement` x:Class sai (`Window1`), `StatisticsWindow` namespace sai (`FootBallCSharp`) |
| 5 | **Bao** | `bao` / `quanLyDoiBong` | `QuanLyDoiBong` | Quản lý đội bóng | `teams` | ⚠️ Đã push XAML, **chưa có code-behind logic** |
| 6 | **QuocDo** | `QuocDo` | `StandingsWindow` | Bảng xếp hạng | `Tournament`, `Team`, `Round`, `Match`, `Standings`, `Player`, `PlayerStatistics` + 7 SPs + data mẫu | ✅ Đã push XAML + code + DB (`FootballManagementDB.sql`) |
| 7 | **Huy** | `Huy` | `ReportWindow` | Báo cáo / Xuất Excel, PDF | *(dùng chung DB)* | ❌ **Thiếu NuGet packages** (ClosedXML, iTextSharp, Entity Framework) |
| 8 | **Chinh** | `chinh` | *(merge commit, không có file mới riêng)* | — | — | ❌ **Chưa push phần riêng** |
| 9 | **Dat** | `Dat` | *(merge commit, không có file mới riêng)* | — | — | ❌ **Chưa push phần riêng** |
| 10 | **Hien** | `Hien` | `LichThiDau` (trong subfolder cũ) | Lịch thi đấu | — | ❌ **Chưa di chuyển vào project chính** |
| — | **MinhQuan** | `MinhQuan` | *(copy lại file của người khác)* | — | — | ❌ **Chưa có phần riêng** |

### 📊 Tổng kết nhanh

| Trạng thái | Số người | Ai |
|------------|----------|-----|
| ✅ Hoàn thành (XAML + code + DB) | **3** | Đức, QuocDo, QuangBao |
| ⚠️ Đã push nhưng chưa hoàn chỉnh | **3** | Việt, TranPhuc, Bao |
| ❌ Chưa push phần riêng / thiếu nhiều | **4+** | Huy, Chinh, Dat, Hien, MinhQuan |

> **Nhánh Git:** `duc`, `chinh`, `Dat`, `Hien`, `Huy`, `MinhQuan`, `QuangBao`, `QuocDo`, `TranPhuc`, `Viet`, `bao`, `quanLyDoiBong`

---

## 🗄️ Database

### File DB riêng (mỗi thành viên tự tạo)
Mỗi người tạo file SQL riêng cho phần của mình (ví dụ: `FootballDB_Schema.sql` của Đức).

### File DB tổng hợp: `FootballDB_All.sql`
Gộp tất cả DB của các thành viên vào **1 file duy nhất**. Hiện tại đã có:

| Phần DB | Bảng | Trạng thái |
|---------|------|------------|
| Quản lý đội bóng | `teams` (team_id, team_name, logo_path, tournament_id, coach_id, status) | ✅ Đã có |
| Quản lý cầu thủ | `players`, `player_general_statistics`, `player_notes`, `player_attachments` | ✅ Đã có |
| Quản lý HLV | `coaches`, `coach_history` | ✅ Đã có |
| Giải đấu (v1) | `Tournaments` | ✅ Đã có |
| Đăng nhập | `Roles`, `Users` | ✅ Đã có |
| Bảng xếp hạng | `Tournament`, `Team`, `Round`, `Match`, `Standings`, `Player`, `PlayerStatistics` + 7 SPs + data mẫu | ✅ Đã có |
| **Kết quả trận đấu (Đức)** | `Teams`, `Matches`, `MatchResults` + `vw_MatchDetails` + 5 SPs + data mẫu | ✅ **Đã gộp** |
| Báo cáo | *(dùng chung DB)* | — |
| Thống kê | *(dùng chung DB)* | — |

---

## ⚠️ Các vấn đề cần giải quyết khi tích hợp

### 1. Namespace không thống nhất
Hiện tại các file XAML dùng **namespace khác nhau**:

| File | Namespace hiện tại | Cần đổi thành |
|------|--------------------|---------------|
| `App.xaml` | `FootBallCSharp` | `Football_Management_System` |
| `StatisticsWindow.xaml` | `FootBallCSharp` | `Football_Management_System` |
| `PlayerManagement.xaml` | `DoAnDotNET` | `Football_Management_System` |
| `LoginWindow.xaml.cs` | `Football_Management_System.Views` | `Football_Management_System` |
| Các file còn lại | `Football_Management_System` | ✅ OK |

### 2. Bảng trùng tên
Trong `FootballDB_All.sql` có nhiều bảng trùng tên (SQL Server **không phân biệt** hoa/thường):
- `teams` xuất hiện **3 lần** (dòng 1, dòng 133, và phần của Đức dùng `Teams`)
- `Tournaments` vs `Tournament` — 2 bảng giải đấu khác cấu trúc

→ **Cần thống nhất**: chọn 1 bảng `teams` dùng chung, 1 bảng `Tournament` dùng chung.

### 3. NuGet Packages thiếu
`ReportWindow.xaml.cs` dùng thư viện chưa cài:
- `ClosedXML` — xuất Excel
- `iTextSharp` — xuất PDF
- `FootballTournamentEntities` — Entity Framework model chưa tạo

→ Thành viên phụ trách `ReportWindow` cần thêm NuGet packages.

### 4. `x:Class` không khớp
`CoachManagementWindow.xaml` khai báo `x:Class="Football_Management_System.Window1"` — cần đổi thành `CoachManagementWindow`.

---

## 🚀 Hướng dẫn chạy

### Bước 1: Clone & checkout nhánh test
```bash
git clone https://github.com/Duc140405/FootBallCSharp.git
cd FootBallCSharp
git checkout test
```

### Bước 2: Tạo database
Mở **SQL Server Management Studio** hoặc **sqlcmd** và chạy:
```sql
-- Nếu dùng LocalDB:
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "Football_Management_System\Database\FootballDB_All.sql"
```

### Bước 3: Kiểm tra connection string
Trong `DataAccess\DatabaseHelper.cs`:
```csharp
// LocalDB:
_connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=FootballManagementDB;Integrated Security=True;";

// SQL Server Express:
// _connectionString = "Server=.\\SQLEXPRESS;Database=FootballManagementDB;Integrated Security=True;";

// SQL Server default:
// _connectionString = "Server=.;Database=FootballManagementDB;Integrated Security=True;";
```

### Bước 4: Build & Run
Mở `Football_Management_System.sln` bằng Visual Studio → Build → Run (F5).

---

## ✅ Phần đã hoàn thành (Nguyễn Tấn Đức)

### Nhánh `duc` — DB riêng + code
- [x] `MatchResultMainWindow.xaml/.cs` — Màn hình chính với nút Start
- [x] `MatchResultWindow.xaml/.cs` — CRUD kết quả trận đấu (thêm/sửa/xóa/tìm kiếm)
- [x] `DataAccess/DatabaseHelper.cs` — Kết nối SQL Server, gọi Stored Procedures
- [x] `Database/FootballDB_Schema.sql` — Schema riêng: 3 bảng + 1 view + 5 SPs + data mẫu
- [x] Hỗ trợ **chế độ offline** khi không có DB

### Nhánh `test` — Tích hợp
- [x] Gộp DB của Đức vào `FootballDB_All.sql` (cuối file, có comment phân biệt)
- [x] Copy `DatabaseHelper.cs` + `FootballDB_Schema.sql` sang nhánh test
- [x] Fix merge conflicts (`App.config`, `MatchResultWindow.xaml`)
- [x] Thêm `DatabaseHelper.cs` vào `.csproj`

---

## 📋 TODO cho các thành viên

- [ ] **Tất cả:** Thống nhất namespace thành `Football_Management_System`
- [ ] **Tất cả:** Thống nhất bảng `teams`/`Teams` dùng chung 1 bảng
- [ ] **Tất cả:** Mỗi người tạo `DataAccess` class riêng hoặc dùng chung `DatabaseHelper.cs`
- [ ] **ReportWindow:** Thêm NuGet packages (ClosedXML, iTextSharp)
- [ ] **CoachManagement:** Sửa `x:Class` từ `Window1` thành `CoachManagementWindow`
- [ ] **LoginWindow:** Đổi namespace từ `Views` về root
- [ ] **App.xaml:** Đổi `StartupUri` thành `LoginWindow.xaml` (hoặc MainWindow khi tích hợp xong)
