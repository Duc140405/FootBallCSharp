-- =============================================
-- FootballDB_Master.sql
-- Database chuẩn duy nhất cho Football Management System
-- Ngày tạo: 2026-02-26
-- Nhóm: 10 thành viên — Nhánh: test
-- =============================================

-- Xóa Database cũ nếu tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FootballManagementDB')
    DROP DATABASE [FootballManagementDB];
GO

-- Tạo Database
CREATE DATABASE [FootballManagementDB];
GO

USE [FootballManagementDB];
GO

-- =============================================
-- PHẦN 1: BẢNG NỀN TẢNG (Không có FK)
-- =============================================

-- 1.1 Giải đấu (Tournaments)
-- Gộp từ: Tournaments (Bao) + Tournament (QuocDo)
CREATE TABLE Tournaments (
    TournamentID INT IDENTITY(1,1) PRIMARY KEY,
    TournamentName NVARCHAR(200) NOT NULL,
    TotalRounds INT DEFAULT 0,
    Season NVARCHAR(50),
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT N'Đang diễn ra',
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE()
);
GO

-- 1.2 Huấn luyện viên (Coaches)
-- Gộp từ: coaches (TranPhuc)
CREATE TABLE Coaches (
    CoachID INT IDENTITY(1,1) PRIMARY KEY,
    CoachName NVARCHAR(150) NOT NULL,
    BirthDate DATE,
    Nationality NVARCHAR(100),
    ExperienceYears INT DEFAULT 0,
    CurrentTeamID INT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- 1.3 Phân quyền (Roles)
-- QuangBao
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- =============================================
-- PHẦN 2: BẢNG CẤP 2 (FK → Phần 1)
-- =============================================

-- 2.1 Đội bóng (Teams)
-- Bảng chuẩn DUY NHẤT — Gộp từ: teams (Bao/Việt) + Team (QuocDo) + Teams (Đức)
CREATE TABLE Teams (
    TeamID INT IDENTITY(1,1) PRIMARY KEY,
    TeamName NVARCHAR(200) NOT NULL,
    ShortName NVARCHAR(50),
    LogoPath NVARCHAR(255),
    Stadium NVARCHAR(200),
    Status NVARCHAR(50),
    TournamentID INT NULL,
    CoachID INT NULL,
    FoundedYear INT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Teams_Tournaments FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
    CONSTRAINT FK_Teams_Coaches FOREIGN KEY (CoachID) REFERENCES Coaches(CoachID)
);
GO

-- 2.2 Người dùng (Users)
-- QuangBao
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    FullName NVARCHAR(100),
    RoleID INT NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,

    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO

-- 2.3 Vòng đấu (Rounds)
-- QuocDo
CREATE TABLE Rounds (
    RoundID INT IDENTITY(1,1) PRIMARY KEY,
    TournamentID INT NOT NULL,
    RoundNumber INT NOT NULL,
    RoundName NVARCHAR(100),
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT N'Chưa bắt đầu',

    CONSTRAINT FK_Rounds_Tournaments FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID)
);
GO

-- =============================================
-- PHẦN 3: BẢNG CẤP 3 (FK → Phần 2)
-- =============================================

-- 3.1 Cầu thủ (Players)
-- Gộp từ: players (Việt) + Player (QuocDo)
CREATE TABLE Players (
    PlayerID INT IDENTITY(1,1) PRIMARY KEY,
    TeamID INT NOT NULL,
    PlayerName NVARCHAR(200) NOT NULL,
    DateOfBirth DATE,
    Nationality NVARCHAR(100),
    JerseyNumber INT,
    Position NVARCHAR(50),
    SubPosition NVARCHAR(50),
    PreferredFoot NVARCHAR(20),
    HeightCm INT,
    WeightKg INT,
    Status NVARCHAR(30) DEFAULT N'Sẵn sàng',
    TechnicalScore INT,
    Photo NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Players_Teams FOREIGN KEY (TeamID) REFERENCES Teams(TeamID),
    CONSTRAINT UQ_Team_JerseyNumber UNIQUE (TeamID, JerseyNumber)
);
GO

-- 3.2 Trận đấu (Matches)
-- Gộp từ: Matches (Đức) + Match (QuocDo)
CREATE TABLE Matches (
    MatchID INT IDENTITY(1,1) PRIMARY KEY,
    TournamentID INT NULL,
    RoundID INT NULL,
    HomeTeamID INT NOT NULL,
    AwayTeamID INT NOT NULL,
    MatchDate DATETIME NOT NULL,
    Stadium NVARCHAR(200),
    Status NVARCHAR(50) DEFAULT N'Chưa diễn ra',
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Matches_Tournaments FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
    CONSTRAINT FK_Matches_Rounds FOREIGN KEY (RoundID) REFERENCES Rounds(RoundID),
    CONSTRAINT FK_Matches_HomeTeam FOREIGN KEY (HomeTeamID) REFERENCES Teams(TeamID),
    CONSTRAINT FK_Matches_AwayTeam FOREIGN KEY (AwayTeamID) REFERENCES Teams(TeamID),
    CONSTRAINT CHK_DifferentTeams CHECK (HomeTeamID != AwayTeamID)
);
GO

-- 3.3 Lịch sử HLV (CoachHistory)
-- TranPhuc
CREATE TABLE CoachHistory (
    HistoryID INT IDENTITY(1,1) PRIMARY KEY,
    CoachID INT NOT NULL,
    TeamID INT NOT NULL,
    FromYear INT NOT NULL,
    ToYear INT NULL,
    Achievement NVARCHAR(255),

    CONSTRAINT FK_CoachHistory_Coaches FOREIGN KEY (CoachID) REFERENCES Coaches(CoachID) ON DELETE CASCADE,
    CONSTRAINT FK_CoachHistory_Teams FOREIGN KEY (TeamID) REFERENCES Teams(TeamID) ON DELETE CASCADE
);
GO

-- =============================================
-- PHẦN 4: BẢNG CẤP 4 (FK → Phần 3)
-- =============================================

-- 4.1 Kết quả trận đấu (MatchResults)
-- Đức
CREATE TABLE MatchResults (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    MatchID INT NOT NULL UNIQUE,
    HomeScore INT DEFAULT 0,
    AwayScore INT DEFAULT 0,
    HomeYellowCards INT DEFAULT 0,
    AwayYellowCards INT DEFAULT 0,
    HomeRedCards INT DEFAULT 0,
    AwayRedCards INT DEFAULT 0,
    Note NVARCHAR(500),

    CONSTRAINT FK_MatchResults_Matches FOREIGN KEY (MatchID) REFERENCES Matches(MatchID) ON DELETE CASCADE,
    CONSTRAINT CHK_HomeScore CHECK (HomeScore >= 0),
    CONSTRAINT CHK_AwayScore CHECK (AwayScore >= 0),
    CONSTRAINT CHK_HomeYellow CHECK (HomeYellowCards >= 0),
    CONSTRAINT CHK_AwayYellow CHECK (AwayYellowCards >= 0),
    CONSTRAINT CHK_HomeRed CHECK (HomeRedCards >= 0),
    CONSTRAINT CHK_AwayRed CHECK (AwayRedCards >= 0)
);
GO

-- 4.2 Bảng xếp hạng (Standings)
-- QuocDo
CREATE TABLE Standings (
    StandingID INT IDENTITY(1,1) PRIMARY KEY,
    TournamentID INT NOT NULL,
    TeamID INT NOT NULL,
    Position INT DEFAULT 0,
    MatchesPlayed INT DEFAULT 0,
    Wins INT DEFAULT 0,
    Draws INT DEFAULT 0,
    Losses INT DEFAULT 0,
    GoalsFor INT DEFAULT 0,
    GoalsAgainst INT DEFAULT 0,
    GoalDifference AS (GoalsFor - GoalsAgainst),
    Points AS (Wins * 3 + Draws * 1),
    UpdatedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Standings_Tournaments FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
    CONSTRAINT FK_Standings_Teams FOREIGN KEY (TeamID) REFERENCES Teams(TeamID),
    CONSTRAINT UQ_Tournament_Team UNIQUE (TournamentID, TeamID)
);
GO

-- 4.3 Thống kê cầu thủ theo trận (PlayerStatistics)
-- QuocDo
CREATE TABLE PlayerStatistics (
    StatID INT IDENTITY(1,1) PRIMARY KEY,
    PlayerID INT NOT NULL,
    TournamentID INT NOT NULL,
    MatchID INT NOT NULL,
    Goals INT DEFAULT 0,
    Assists INT DEFAULT 0,
    YellowCards INT DEFAULT 0,
    RedCards INT DEFAULT 0,
    MinutesPlayed INT DEFAULT 0,

    CONSTRAINT FK_PlayerStats_Players FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID),
    CONSTRAINT FK_PlayerStats_Tournaments FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
    CONSTRAINT FK_PlayerStats_Matches FOREIGN KEY (MatchID) REFERENCES Matches(MatchID)
);
GO

-- 4.4 Thống kê tổng cầu thủ (PlayerGeneralStatistics)
-- Việt
CREATE TABLE PlayerGeneralStatistics (
    PlayerID INT PRIMARY KEY,
    Matches INT DEFAULT 0,
    Goals INT DEFAULT 0,
    Assists INT DEFAULT 0,
    YellowCards INT DEFAULT 0,
    RedCards INT DEFAULT 0,

    CONSTRAINT FK_PlayerGeneralStats_Players FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE
);
GO

-- 4.5 Ghi chú cầu thủ (PlayerNotes)
-- Việt
CREATE TABLE PlayerNotes (
    NoteID INT IDENTITY(1,1) PRIMARY KEY,
    PlayerID INT NOT NULL,
    NoteContent NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_PlayerNotes_Players FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE
);
GO

-- 4.6 Đính kèm cầu thủ (PlayerAttachments)
-- Việt
CREATE TABLE PlayerAttachments (
    AttachmentID INT IDENTITY(1,1) PRIMARY KEY,
    PlayerID INT NOT NULL,
    FileName NVARCHAR(255),
    FilePath NVARCHAR(255),
    UploadedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_PlayerAttachments_Players FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE
);
GO

-- =============================================
-- PHẦN 5: VIEWS
-- =============================================

-- 5.1 View chi tiết trận đấu (Đức)
CREATE VIEW vw_MatchDetails AS
SELECT 
    m.MatchID AS MatchId,
    ht.TeamName AS HomeTeam,
    at2.TeamName AS AwayTeam,
    m.MatchDate,
    mr.HomeScore,
    mr.AwayScore,
    mr.HomeYellowCards,
    mr.AwayYellowCards,
    mr.HomeRedCards,
    mr.AwayRedCards,
    mr.Note
FROM Matches m
INNER JOIN Teams ht ON m.HomeTeamID = ht.TeamID
INNER JOIN Teams at2 ON m.AwayTeamID = at2.TeamID
LEFT JOIN MatchResults mr ON m.MatchID = mr.MatchID;
GO

-- =============================================
-- PHẦN 6: STORED PROCEDURES
-- =============================================

-- 6.1 SP: Thêm trận đấu (Đức)
CREATE PROCEDURE sp_AddMatch
    @HomeTeam NVARCHAR(100),
    @AwayTeam NVARCHAR(100),
    @MatchDate DATETIME,
    @HomeScore INT = NULL,
    @AwayScore INT = NULL,
    @HomeYellowCards INT = NULL,
    @AwayYellowCards INT = NULL,
    @HomeRedCards INT = NULL,
    @AwayRedCards INT = NULL,
    @Note NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @HomeTeamID INT, @AwayTeamID INT, @MatchID INT;

    SELECT @HomeTeamID = TeamID FROM Teams WHERE TeamName = @HomeTeam;
    IF @HomeTeamID IS NULL
    BEGIN
        INSERT INTO Teams (TeamName) VALUES (@HomeTeam);
        SET @HomeTeamID = SCOPE_IDENTITY();
    END

    SELECT @AwayTeamID = TeamID FROM Teams WHERE TeamName = @AwayTeam;
    IF @AwayTeamID IS NULL
    BEGIN
        INSERT INTO Teams (TeamName) VALUES (@AwayTeam);
        SET @AwayTeamID = SCOPE_IDENTITY();
    END

    INSERT INTO Matches (HomeTeamID, AwayTeamID, MatchDate)
    VALUES (@HomeTeamID, @AwayTeamID, @MatchDate);
    SET @MatchID = SCOPE_IDENTITY();

    IF @HomeScore IS NOT NULL OR @AwayScore IS NOT NULL
    BEGIN
        INSERT INTO MatchResults (MatchID, HomeScore, AwayScore,
            HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards, Note)
        VALUES (@MatchID, ISNULL(@HomeScore, 0), ISNULL(@AwayScore, 0),
            ISNULL(@HomeYellowCards, 0), ISNULL(@AwayYellowCards, 0),
            ISNULL(@HomeRedCards, 0), ISNULL(@AwayRedCards, 0), @Note);
    END

    SELECT @MatchID AS NewMatchId;
END
GO

-- 6.2 SP: Cập nhật kết quả (Đức)
CREATE PROCEDURE sp_UpdateMatchResult
    @MatchId INT,
    @HomeScore INT,
    @AwayScore INT,
    @HomeYellowCards INT = 0,
    @AwayYellowCards INT = 0,
    @HomeRedCards INT = 0,
    @AwayRedCards INT = 0,
    @Note NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM MatchResults WHERE MatchID = @MatchId)
    BEGIN
        UPDATE MatchResults
        SET HomeScore = @HomeScore,
            AwayScore = @AwayScore,
            HomeYellowCards = @HomeYellowCards,
            AwayYellowCards = @AwayYellowCards,
            HomeRedCards = @HomeRedCards,
            AwayRedCards = @AwayRedCards,
            Note = @Note
        WHERE MatchID = @MatchId;
    END
    ELSE
    BEGIN
        INSERT INTO MatchResults (MatchID, HomeScore, AwayScore,
            HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards, Note)
        VALUES (@MatchId, @HomeScore, @AwayScore,
            @HomeYellowCards, @AwayYellowCards, @HomeRedCards, @AwayRedCards,
            @Note);
    END
END
GO

-- 6.3 SP: Xóa trận đấu (Đức)
CREATE PROCEDURE sp_DeleteMatch
    @MatchId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Matches WHERE MatchID = @MatchId;
END
GO

-- 6.4 SP: Tìm kiếm trận đấu (Đức)
CREATE PROCEDURE sp_SearchMatches
    @Keyword NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM vw_MatchDetails
    WHERE HomeTeam LIKE '%' + @Keyword + '%'
       OR AwayTeam LIKE '%' + @Keyword + '%'
    ORDER BY MatchDate DESC;
END
GO

-- 6.5 SP: Thống kê trận đấu (Đức)
CREATE PROCEDURE sp_GetStatistics
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        COUNT(*) AS TongTran,
        SUM(CASE WHEN mr.ResultID IS NOT NULL THEN 1 ELSE 0 END) AS DaCoKetQua,
        SUM(CASE WHEN mr.ResultID IS NULL THEN 1 ELSE 0 END) AS ChuaCoKetQua
    FROM Matches m
    LEFT JOIN MatchResults mr ON m.MatchID = mr.MatchID;
END
GO

-- 6.6 SP: Lấy bảng xếp hạng (QuocDo)
CREATE PROCEDURE sp_GetStandings
    @TournamentID INT = NULL,
    @RoundID INT = NULL
AS
BEGIN
    SELECT s.Position AS [STT], t.TeamName AS N'Đội Bóng', s.MatchesPlayed AS N'Số Trận',
           s.Wins AS N'Thắng', s.Draws AS N'Hòa', s.Losses AS N'Thua',
           s.GoalsFor AS [BT], s.GoalsAgainst AS [BB], s.GoalDifference AS [HS], s.Points AS N'Điểm'
    FROM Standings s INNER JOIN Teams t ON s.TeamID = t.TeamID
    WHERE (@TournamentID IS NULL OR s.TournamentID = @TournamentID)
    ORDER BY s.Points DESC, s.GoalDifference DESC, s.GoalsFor DESC, t.TeamName ASC;
END
GO

-- 6.7 SP: Tính lại vị trí BXH (QuocDo) — phải tạo TRƯỚC sp_UpdateStandingsAfterMatch
CREATE PROCEDURE sp_RecalculateStandings @TournamentID INT
AS
BEGIN
    WITH RankedStandings AS (
        SELECT StandingID, ROW_NUMBER() OVER (ORDER BY Points DESC, GoalDifference DESC, GoalsFor DESC, TeamID ASC) AS NewPosition
        FROM Standings WHERE TournamentID = @TournamentID
    )
    UPDATE s SET s.Position = rs.NewPosition, s.UpdatedDate = GETDATE()
    FROM Standings s INNER JOIN RankedStandings rs ON s.StandingID = rs.StandingID;
END
GO

-- 6.8 SP: Cập nhật BXH sau trận đấu (QuocDo)
CREATE PROCEDURE sp_UpdateStandingsAfterMatch @MatchID INT
AS
BEGIN
    DECLARE @TournamentID INT, @HomeTeamID INT, @AwayTeamID INT, @HomeScore INT, @AwayScore INT;

    SELECT @TournamentID = m.TournamentID, @HomeTeamID = m.HomeTeamID, @AwayTeamID = m.AwayTeamID,
           @HomeScore = mr.HomeScore, @AwayScore = mr.AwayScore
    FROM Matches m
    INNER JOIN MatchResults mr ON m.MatchID = mr.MatchID
    WHERE m.MatchID = @MatchID AND mr.HomeScore IS NOT NULL AND mr.AwayScore IS NOT NULL;

    IF @TournamentID IS NULL RETURN;

    IF NOT EXISTS (SELECT 1 FROM Standings WHERE TournamentID = @TournamentID AND TeamID = @HomeTeamID)
        INSERT INTO Standings (TournamentID, TeamID, Position) VALUES (@TournamentID, @HomeTeamID, 0);

    UPDATE Standings SET MatchesPlayed = MatchesPlayed + 1,
        Wins = Wins + CASE WHEN @HomeScore > @AwayScore THEN 1 ELSE 0 END,
        Draws = Draws + CASE WHEN @HomeScore = @AwayScore THEN 1 ELSE 0 END,
        Losses = Losses + CASE WHEN @HomeScore < @AwayScore THEN 1 ELSE 0 END,
        GoalsFor = GoalsFor + @HomeScore, GoalsAgainst = GoalsAgainst + @AwayScore, UpdatedDate = GETDATE()
    WHERE TournamentID = @TournamentID AND TeamID = @HomeTeamID;

    IF NOT EXISTS (SELECT 1 FROM Standings WHERE TournamentID = @TournamentID AND TeamID = @AwayTeamID)
        INSERT INTO Standings (TournamentID, TeamID, Position) VALUES (@TournamentID, @AwayTeamID, 0);

    UPDATE Standings SET MatchesPlayed = MatchesPlayed + 1,
        Wins = Wins + CASE WHEN @AwayScore > @HomeScore THEN 1 ELSE 0 END,
        Draws = Draws + CASE WHEN @AwayScore = @HomeScore THEN 1 ELSE 0 END,
        Losses = Losses + CASE WHEN @AwayScore < @HomeScore THEN 1 ELSE 0 END,
        GoalsFor = GoalsFor + @AwayScore, GoalsAgainst = GoalsAgainst + @HomeScore, UpdatedDate = GETDATE()
    WHERE TournamentID = @TournamentID AND TeamID = @AwayTeamID;

    EXEC sp_RecalculateStandings @TournamentID;
END
GO

-- 6.9 SP: Tìm đội trong BXH (QuocDo)
CREATE PROCEDURE sp_SearchTeamInStandings @TournamentID INT, @SearchText NVARCHAR(200)
AS
BEGIN
    SELECT s.Position AS [STT], t.TeamName AS N'Đội Bóng', s.MatchesPlayed AS N'Số Trận',
           s.Wins AS N'Thắng', s.Draws AS N'Hòa', s.Losses AS N'Thua',
           s.GoalsFor AS [BT], s.GoalsAgainst AS [BB], s.GoalDifference AS [HS], s.Points AS N'Điểm'
    FROM Standings s INNER JOIN Teams t ON s.TeamID = t.TeamID
    WHERE s.TournamentID = @TournamentID AND t.TeamName LIKE N'%' + @SearchText + '%'
    ORDER BY s.Points DESC, s.GoalDifference DESC, s.GoalsFor DESC, t.TeamName ASC;
END
GO

-- 6.10 SP: Lấy danh sách giải đấu (QuocDo)
CREATE PROCEDURE sp_GetTournaments
AS
BEGIN
    SELECT TournamentID, TournamentName, Season, StartDate, EndDate, Status
    FROM Tournaments ORDER BY StartDate DESC;
END
GO

-- 6.11 SP: Lấy vòng đấu theo giải (QuocDo)
CREATE PROCEDURE sp_GetRoundsByTournament @TournamentID INT
AS
BEGIN
    SELECT RoundID, RoundNumber, RoundName, StartDate, EndDate, Status
    FROM Rounds WHERE TournamentID = @TournamentID ORDER BY RoundNumber ASC;
END
GO

-- 6.12 SP: Reset BXH (QuocDo)
CREATE PROCEDURE sp_ResetStandings @TournamentID INT
AS
BEGIN
    UPDATE Standings SET MatchesPlayed = 0, Wins = 0, Draws = 0, Losses = 0,
        GoalsFor = 0, GoalsAgainst = 0, Position = 0, UpdatedDate = GETDATE()
    WHERE TournamentID = @TournamentID;
END
GO

-- =============================================
-- PHẦN 6B: STORED PROCEDURES - LOGIN & REGISTER
-- =============================================

-- 6.13 SP: Kiểm tra login (Đăng nhập)
CREATE PROCEDURE sp_Login
    @Username NVARCHAR(50),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Username, Email, FullName, RoleID, IsActive
    FROM Users
    WHERE Username = @Username 
      AND PasswordHash = @Password 
      AND IsActive = 1;
END
GO

-- 6.14 SP: Đăng ký user mới
CREATE PROCEDURE sp_RegisterUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @Email NVARCHAR(100),
    @FullName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra username đã tồn tại
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        SELECT 0 AS Result, N'Username đã tồn tại!' AS Message;
        RETURN;
    END
    
    -- Kiểm tra email đã tồn tại
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        SELECT 0 AS Result, N'Email đã được dùng!' AS Message;
        RETURN;
    END
    
    -- Thêm user mới (RoleID = 2: User)
    BEGIN TRY
        INSERT INTO Users (Username, PasswordHash, Email, FullName, RoleID, IsActive)
        VALUES (@Username, @Password, @Email, @FullName, 2, 1);
        
        SELECT 1 AS Result, N'Đăng ký thành công!' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Result, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- 6.15 SP: Kiểm tra username có tồn tại
CREATE PROCEDURE sp_CheckUsernameExists
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS UserCount
    FROM Users
    WHERE Username = @Username;
END
GO

-- 6.16 SP: Lấy thông tin user
CREATE PROCEDURE sp_GetUserInfo
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Username, Email, FullName, RoleID, CreatedDate, IsActive
    FROM Users
    WHERE UserID = @UserID;
END
GO

-- =============================================
-- PHẦN 7: DỮ LIỆU MẪU (Sample Data)
-- =============================================

-- 7.0 Tạo hàm hash password SHA256
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'FN' AND name = 'fn_HashPassword')
    DROP FUNCTION dbo.fn_HashPassword
GO

CREATE FUNCTION dbo.fn_HashPassword(@Password NVARCHAR(255))
RETURNS NVARCHAR(64)
AS
BEGIN
    RETURN CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', @Password), 2)
END
GO

-- 7.1 Roles + Users (QuangBao)
INSERT INTO Roles (RoleName) VALUES (N'Admin'), (N'User'), (N'Manager');
INSERT INTO Users (Username, PasswordHash, Email, FullName, RoleID)
VALUES (N'admin', dbo.fn_HashPassword('123'), N'admin@football.com', N'Administrator', 1),
       (N'user', dbo.fn_HashPassword('123'), N'user@football.com', N'User', 2);
GO

-- 7.2 Giải đấu
INSERT INTO Tournaments (TournamentName, Season, StartDate, EndDate, Status) VALUES 
    (N'Giải VĐQG 2026', N'2026', '2026-01-01', '2026-12-31', N'Đang diễn ra'),
    (N'Cup Sinh viên 2026', N'2026', '2026-03-01', '2026-06-30', N'Đang diễn ra');
GO

-- 7.3 HLV
INSERT INTO Coaches (CoachName, Nationality, ExperienceYears) VALUES
    (N'Nguyễn Văn A', N'Việt Nam', 10),
    (N'Trần Văn B', N'Việt Nam', 8),
    (N'Lê Văn C', N'Việt Nam', 5);
GO

-- 7.4 Đội bóng
INSERT INTO Teams (TeamName, ShortName, Stadium, CoachID, TournamentID) VALUES 
    (N'Hà Nội FC', N'HN', N'Sân Hàng Đẫy', 1, 1),
    (N'Hoàng Anh Gia Lai', N'HAGL', N'Sân Pleiku', 2, 1),
    (N'Sài Gòn FC', N'SG', N'Sân Thống Nhất', 3, 1),
    (N'Viettel FC', N'VT', N'Sân Hàng Đẫy', NULL, 1),
    (N'Thanh Hóa FC', N'TH', N'Sân Thanh Hóa', NULL, 1),
    (N'Manchester United', NULL, NULL, NULL, NULL),
    (N'Manchester City', NULL, NULL, NULL, NULL),
    (N'Liverpool', NULL, NULL, NULL, NULL),
    (N'Chelsea', NULL, NULL, NULL, NULL),
    (N'Arsenal', NULL, NULL, NULL, NULL),
    (N'Tottenham', NULL, NULL, NULL, NULL);
GO

-- 7.5 Vòng đấu (QuocDo)
INSERT INTO Rounds (TournamentID, RoundNumber, RoundName, Status) VALUES 
    (1, 1, N'Vòng 1', N'Hoàn thành'),
    (1, 2, N'Vòng 2', N'Hoàn thành'),
    (1, 3, N'Vòng 3', N'Đang diễn ra');
GO

-- 7.6 Trận đấu mẫu (Đức) — TeamID 6-11 = Manchester United..Tottenham
INSERT INTO Matches (HomeTeamID, AwayTeamID, MatchDate) VALUES
    (6, 7, '2026-03-01 19:00:00'),
    (8, 9, '2026-03-02 21:00:00'),
    (10, 11, '2026-03-03 18:30:00');
GO

-- 7.7 Kết quả mẫu (Đức)
INSERT INTO MatchResults (MatchID, HomeScore, AwayScore, HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards, Note)
VALUES (1, 2, 1, 3, 2, 0, 1, N'Trận derby Manchester');
GO

-- 7.8 BXH mẫu (QuocDo) — TeamID 1-5 = Hà Nội..Thanh Hóa
INSERT INTO Standings (TournamentID, TeamID, Position, MatchesPlayed, Wins, Draws, Losses, GoalsFor, GoalsAgainst) VALUES 
    (1, 1, 1, 2, 2, 0, 0, 5, 1),
    (1, 2, 2, 2, 1, 1, 0, 4, 2),
    (1, 3, 3, 2, 1, 0, 1, 3, 3),
    (1, 4, 4, 2, 0, 1, 1, 2, 4),
    (1, 5, 5, 2, 0, 0, 2, 1, 5);
GO

-- =============================================
-- 7.9 CẦU THỦ MẪU (Việt)
-- TeamID 1-5 = Hà Nội..Thanh Hóa
-- =============================================

INSERT INTO Players 
(TeamID, PlayerName, DateOfBirth, Nationality,
 JerseyNumber, Position, SubPosition,
 PreferredFoot, HeightCm, WeightKg,
 Status, TechnicalScore)
VALUES
(1, N'Nguyễn Quang Hải', '1997-04-12', N'Việt Nam',
 19, N'MF', N'Attacking Midfielder',
 N'Trái', 168, 65,
 N'Sẵn sàng', 9),

(1, N'Phạm Tuấn Hải', '1998-05-19', N'Việt Nam',
 9, N'FW', N'Striker',
 N'Phải', 178, 72,
 N'Sẵn sàng', 8),

(2, N'Nguyễn Công Phượng', '1995-01-21', N'Việt Nam',
 10, N'FW', N'Second Striker',
 N'Phải', 168, 63,
 N'Sẵn sàng', 8),

(3, N'Đỗ Hùng Dũng', '1993-09-08', N'Việt Nam',
 88, N'MF', N'Central Midfielder',
 N'Phải', 170, 67,
 N'Sẵn sàng', 8),

(4, N'Bùi Tiến Dũng', '1995-10-02', N'Việt Nam',
 1, N'GK', N'Goalkeeper',
 N'Phải', 181, 78,
 N'Sẵn sàng', 7),

(5, N'Đoàn Văn Hậu', '1999-04-19', N'Việt Nam',
 5, N'DF', N'Left Back',
 N'Trái', 185, 79,
 N'Sẵn sàng', 8);
GO

-- =============================================
-- 7.10 THỐNG KÊ TỔNG CẦU THỦ
-- =============================================

INSERT INTO PlayerGeneralStatistics (PlayerID)
SELECT PlayerID FROM Players;
GO

-- =============================================
-- 7.11 GHI CHÚ CẦU THỦ
-- =============================================

INSERT INTO PlayerNotes (PlayerID, NoteContent)
VALUES
(1, N'Cầu thủ có kỹ thuật tốt và khả năng sút xa nguy hiểm.'),
(3, N'Từng thi đấu ở nước ngoài.'),
(6, N'Hậu vệ có thể lực và khả năng tranh chấp mạnh.');
GO

-- =============================================
-- 7.12 FILE ĐÍNH KÈM CẦU THỦ
-- =============================================

INSERT INTO PlayerAttachments (PlayerID, FileName, FilePath)
VALUES
(1, N'quang_hai_profile.pdf', N'/files/quang_hai_profile.pdf'),
(4, N'hung_dung_stats.xlsx', N'/files/hung_dung_stats.xlsx');
GO

PRINT N'=============================================';
PRINT N'✅ FootballDB_Master.sql — Tạo database thành công!';
PRINT N'=============================================';
GO