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

 1. Quan       - LoginWindow                - Dang nhap he thong, phan quyen Admin va Nguoi quan ly, kiem tra hop le tai khoan
 2. Chinh      - QuanLyGiaiDau              - Them, sua, xoa giai dau, thiet lap so vong dau, thoi gian bat dau va ket thuc
 3. Bao        - QuanLyDoiBong              - Them, sua, xoa doi bong, thong tin doi (ten, logo, HLV), gan doi vao giai dau
 4. Viet       - PlayerManagement           - Them, sua, xoa cau thu, so ao, vi tri, ngay sinh, gan cau thu cho doi
 5. Dat        - CoachManagementWindow      - Thong tin HLV, gan HLV cho doi bong, theo doi lich su dan dat
 6. Hien       - LichThiDau                 - Tao lich thi dau tu dong, chinh sua lich, hien thi lich theo vong hoac doi
 7. Duc        - MatchResultWindow          - Nhap ket qua tran, ban thang, the phat, cap nhat ket qua vao he thong
 8. Do         - StandingsWindow            - Tu dong tinh diem, sap xep theo diem va hieu so, cap nhat theo thoi gian thuc
 9. Phuc       - StatisticsWindow           - Vua pha luoi, cau thu kien tao, thong ke so ban thang va the
10. Huy        - ReportWindow               - Xuat bao cao PDF va Excel, thong ke tong quan giai dau, ho tro in an
