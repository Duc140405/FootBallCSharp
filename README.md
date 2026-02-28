Football Management System - WPF

Mon: Lap trinh ung dung .NET
Truong: Dai hoc Duy Tan
Ngay: 26/02/2026
Nhom 3: 10 thanh vien
De tai: Quan ly he thong giai dau bong da
Cong nghe: WPF, Entity Framework 6, SQL Server


CAU TRUC DU AN

Football_Management_System/
    App.xaml / App.xaml.cs
    App.config

    DataAccess/
        FootballDbContext.cs

    Models/
        Tournament.cs
        Team.cs
        Coach.cs
        CoachHistory.cs
        Player.cs
        Match.cs
        MatchResult.cs
        Round.cs
        Standing.cs
        PlayerRelated.cs
        UserRole.cs

    Database/
        FootballDB_Master.sql
        FootballDB_Schema.sql
        FootballDB_All.sql

    Properties/
        AssemblyInfo.cs

    LoginWindow.xaml / .cs
    QuanLyGiaiDau.xaml / .cs
    QuanLyDoiBong.xaml / .cs
    PlayerManagement.xaml / .cs
    CoachManagementWindow.xaml / .cs
    LichThiDau.xaml / .cs
    MatchResultMainWindow.xaml / .cs
    MatchResultWindow.xaml / .cs
    StandingsWindow.xaml / .cs
    StatisticsWindow.xaml / .cs
    ReportWindow.xaml / .cs

    Statistics/
        Models/
            AwardModel.cs
            PlayerStatistic.cs
            TournamentStatistic.cs
        ViewModels/
            StatisticsViewModel.cs
        Views/
            StatisticsAndAwardsWindow.xaml / .cs

    Football_Management_System.csproj
    packages.config


PHAN CONG NHIEM VU CHI TIET

Quan (thanh vien 1)
Window: Dang nhap va phan quyen
- Dang nhap he thong
- Phan quyen Admin / Nguoi quan ly
- Kiem tra hop le tai khoan

Chinh (thanh vien 2)
Window: Quan ly giai dau
- Them / sua / xoa giai dau
- Thiet lap so vong dau
- Thoi gian bat dau va ket thuc

Bao (thanh vien 3)
Window: Quan ly doi bong
- Them / sua / xoa doi bong
- Thong tin doi (ten, logo, HLV)
- Gan doi vao giai dau

Viet (thanh vien 4)
Window: Quan ly cau thu
- Them / sua / xoa cau thu
- So ao, vi tri, ngay sinh
- Gan cau thu cho doi

Dat (thanh vien 5)
Window: Quan ly huan luyen vien
- Thong tin HLV
- Gan HLV cho doi bong
- Theo doi lich su dan dat

Hien (thanh vien 6)
Window: Lich thi dau
- Tao lich thi dau tu dong
- Chinh sua lich
- Hien thi lich theo vong / doi

Duc (thanh vien 7)
Window: Ket qua tran dau
- Nhap ket qua tran
- Ban thang, the phat
- Cap nhat ket qua vao he thong

Do (thanh vien 8)
Window: Bang xep hang
- Tu dong tinh diem
- Sap xep theo diem, hieu so
- Cap nhat theo thoi gian thuc

Phuc (thanh vien 9)
Window: Thong ke va giai thuong
- Vua pha luoi
- Cau thu kien tao
- Thong ke so ban thang, the

Huy (thanh vien 10)
Window: Bao cao va xuat du lieu
- Xuat bao cao PDF / Excel
- Thong ke tong quan giai dau
- Ho tro in an
