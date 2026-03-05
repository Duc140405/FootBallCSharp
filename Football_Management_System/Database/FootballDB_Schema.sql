-- ============================================
-- FOOTBALL MANAGEMENT SYSTEM DATABASE SCHEMA
-- Phần: Match Result Window
-- Author: Nguyễn Tấn Đức
-- Date: 25/02/2026
-- ============================================

-- Tạo Database
CREATE DATABASE FootballManagementDB;
GO

USE FootballManagementDB;
GO

-- ============================================
-- BẢNG TEAMS - Quản lý đội bóng
-- ============================================
CREATE TABLE Teams (
    TeamId INT IDENTITY(1,1) PRIMARY KEY,
    TeamName NVARCHAR(100) NOT NULL
);
GO

-- ============================================
-- BẢNG MATCHES - Quản lý trận đấu
-- ============================================
CREATE TABLE Matches (
    MatchId INT IDENTITY(1,1) PRIMARY KEY,
    HomeTeamId INT NOT NULL,
    AwayTeamId INT NOT NULL,
    MatchDate DATETIME NOT NULL,

    -- Foreign Keys
    CONSTRAINT FK_Matches_HomeTeam FOREIGN KEY (HomeTeamId) REFERENCES Teams(TeamId),
    CONSTRAINT FK_Matches_AwayTeam FOREIGN KEY (AwayTeamId) REFERENCES Teams(TeamId),
    
    -- Đảm bảo đội nhà và đội khách khác nhau
    CONSTRAINT CHK_DifferentTeams CHECK (HomeTeamId != AwayTeamId)
);
GO

-- ============================================
-- BẢNG MATCH_RESULTS - Kết quả trận đấu
-- ============================================
CREATE TABLE MatchResults (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    MatchId INT NOT NULL UNIQUE,
    
    -- Tỷ số
    HomeScore INT DEFAULT 0,
    AwayScore INT DEFAULT 0,
    
    -- Thẻ phạt
    HomeYellowCards INT DEFAULT 0,
    AwayYellowCards INT DEFAULT 0,
    HomeRedCards INT DEFAULT 0,
    AwayRedCards INT DEFAULT 0,
    
    -- Ghi chú
    Note NVARCHAR(500),

    -- Foreign Key
    CONSTRAINT FK_MatchResults_Matches FOREIGN KEY (MatchId) REFERENCES Matches(MatchId) ON DELETE CASCADE,
    
    -- Constraints
    CONSTRAINT CHK_HomeScore CHECK (HomeScore >= 0),
    CONSTRAINT CHK_AwayScore CHECK (AwayScore >= 0),
    CONSTRAINT CHK_HomeYellow CHECK (HomeYellowCards >= 0),
    CONSTRAINT CHK_AwayYellow CHECK (AwayYellowCards >= 0),
    CONSTRAINT CHK_HomeRed CHECK (HomeRedCards >= 0),
    CONSTRAINT CHK_AwayRed CHECK (AwayRedCards >= 0)
);
GO

-- ============================================
-- VIEW: Chi tiết trận đấu với kết quả
-- ============================================
-- (Trả về đúng các cột mà DatabaseHelper.MapToMatch() đọc)
CREATE VIEW vw_MatchDetails AS
SELECT 
    m.MatchId,
    ht.TeamName AS HomeTeam,
    at.TeamName AS AwayTeam,
    m.MatchDate,
    mr.HomeScore,
    mr.AwayScore,
    mr.HomeYellowCards,
    mr.AwayYellowCards,
    mr.HomeRedCards,
    mr.AwayRedCards,
    mr.Note
FROM Matches m
INNER JOIN Teams ht ON m.HomeTeamId = ht.TeamId
INNER JOIN Teams at ON m.AwayTeamId = at.TeamId
LEFT JOIN MatchResults mr ON m.MatchId = mr.MatchId;
GO

-- ============================================
-- STORED PROCEDURES
-- ============================================

-- SP: Thêm trận đấu mới
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
    
    DECLARE @HomeTeamId INT, @AwayTeamId INT, @MatchId INT;
    
    -- Tìm hoặc tạo đội nhà
    SELECT @HomeTeamId = TeamId FROM Teams WHERE TeamName = @HomeTeam;
    IF @HomeTeamId IS NULL
    BEGIN
        INSERT INTO Teams (TeamName) VALUES (@HomeTeam);
        SET @HomeTeamId = SCOPE_IDENTITY();
    END
    
    -- Tìm hoặc tạo đội khách
    SELECT @AwayTeamId = TeamId FROM Teams WHERE TeamName = @AwayTeam;
    IF @AwayTeamId IS NULL
    BEGIN
        INSERT INTO Teams (TeamName) VALUES (@AwayTeam);
        SET @AwayTeamId = SCOPE_IDENTITY();
    END
    
    -- Thêm trận đấu
    INSERT INTO Matches (HomeTeamId, AwayTeamId, MatchDate)
    VALUES (@HomeTeamId, @AwayTeamId, @MatchDate);
    
    SET @MatchId = SCOPE_IDENTITY();
    
    -- Thêm kết quả nếu có
    IF @HomeScore IS NOT NULL OR @AwayScore IS NOT NULL
    BEGIN
        INSERT INTO MatchResults (MatchId, HomeScore, AwayScore, 
            HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards, Note)
        VALUES (@MatchId, ISNULL(@HomeScore, 0), ISNULL(@AwayScore, 0),
            ISNULL(@HomeYellowCards, 0), ISNULL(@AwayYellowCards, 0),
            ISNULL(@HomeRedCards, 0), ISNULL(@AwayRedCards, 0), @Note);
    END
    
    SELECT @MatchId AS NewMatchId;
END
GO

-- SP: Cập nhật kết quả trận đấu
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
    
    IF EXISTS (SELECT 1 FROM MatchResults WHERE MatchId = @MatchId)
    BEGIN
        UPDATE MatchResults
        SET HomeScore = @HomeScore,
            AwayScore = @AwayScore,
            HomeYellowCards = @HomeYellowCards,
            AwayYellowCards = @AwayYellowCards,
            HomeRedCards = @HomeRedCards,
            AwayRedCards = @AwayRedCards,
            Note = @Note
        WHERE MatchId = @MatchId;
    END
    ELSE
    BEGIN
        INSERT INTO MatchResults (MatchId, HomeScore, AwayScore, 
            HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards, Note)
        VALUES (@MatchId, @HomeScore, @AwayScore,
            @HomeYellowCards, @AwayYellowCards, @HomeRedCards, @AwayRedCards, 
            @Note);
    END
END
GO

-- SP: Xóa trận đấu
CREATE PROCEDURE sp_DeleteMatch
    @MatchId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Matches WHERE MatchId = @MatchId;
END
GO

-- SP: Tìm kiếm trận đấu theo tên đội
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

-- SP: Lấy thống kê
CREATE PROCEDURE sp_GetStatistics
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TongTran,
        SUM(CASE WHEN mr.ResultId IS NOT NULL THEN 1 ELSE 0 END) AS DaCoKetQua,
        SUM(CASE WHEN mr.ResultId IS NULL THEN 1 ELSE 0 END) AS ChuaCoKetQua
    FROM Matches m
    LEFT JOIN MatchResults mr ON m.MatchId = mr.MatchId;
END
GO

-- ============================================
-- DỮ LIỆU MẪU
-- ============================================
INSERT INTO Teams (TeamName) VALUES 
(N'Manchester United'),
(N'Manchester City'),
(N'Liverpool'),
(N'Chelsea'),
(N'Arsenal'),
(N'Tottenham');
GO

-- Thêm một số trận đấu mẫu
INSERT INTO Matches (HomeTeamId, AwayTeamId, MatchDate) VALUES
(1, 2, '2026-03-01 19:00:00'),
(3, 4, '2026-03-02 21:00:00'),
(5, 6, '2026-03-03 18:30:00');
GO

-- Thêm kết quả mẫu
INSERT INTO MatchResults (MatchId, HomeScore, AwayScore, HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards, Note)
VALUES (1, 2, 1, 3, 2, 0, 1, N'Trận derby Manchester');
GO

-- Kiểm tra dữ liệu
SELECT * FROM vw_MatchDetails;
GO
