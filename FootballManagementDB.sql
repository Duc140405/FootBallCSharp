-- =============================================
-- Football Management System Database
-- Hệ thống quản lý bảng xếp hạng giải đấu bóng đá
-- =============================================

-- Tạo database
CREATE DATABASE FootballManagementDB;
GO

USE FootballManagementDB;
GO

-- =============================================
-- PHẦN 1: TẠO CÁC BẢNG (TABLES)
-- =============================================

-- Bảng Giải đấu
CREATE TABLE Tournament (
    TournamentID INT PRIMARY KEY IDENTITY(1,1),
    TournamentName NVARCHAR(200) NOT NULL,
    Season NVARCHAR(50),
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT N'Đang diễn ra',
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng Đội bóng
CREATE TABLE Team (
    TeamID INT PRIMARY KEY IDENTITY(1,1),
    TeamName NVARCHAR(200) NOT NULL,
    ShortName NVARCHAR(50),
    Stadium NVARCHAR(200),
    Coach NVARCHAR(100),
    FoundedYear INT,
    Logo NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng Vòng đấu
CREATE TABLE Round (
    RoundID INT PRIMARY KEY IDENTITY(1,1),
    TournamentID INT FOREIGN KEY REFERENCES Tournament(TournamentID),
    RoundNumber INT NOT NULL,
    RoundName NVARCHAR(100),
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT N'Chưa bắt đầu'
);

-- Bảng Trận đấu
CREATE TABLE Match (
    MatchID INT PRIMARY KEY IDENTITY(1,1),
    TournamentID INT FOREIGN KEY REFERENCES Tournament(TournamentID),
    RoundID INT FOREIGN KEY REFERENCES Round(RoundID),
    HomeTeamID INT FOREIGN KEY REFERENCES Team(TeamID),
    AwayTeamID INT FOREIGN KEY REFERENCES Team(TeamID),
    MatchDate DATETIME,
    Stadium NVARCHAR(200),
    HomeScore INT,
    AwayScore INT,
    HomeYellowCards INT DEFAULT 0,
    AwayYellowCards INT DEFAULT 0,
    HomeRedCards INT DEFAULT 0,
    AwayRedCards INT DEFAULT 0,
    Status NVARCHAR(50) DEFAULT N'Chưa diễn ra',
    Note NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng xếp hạng
CREATE TABLE Standings (
    StandingID INT PRIMARY KEY IDENTITY(1,1),
    TournamentID INT FOREIGN KEY REFERENCES Tournament(TournamentID),
    TeamID INT FOREIGN KEY REFERENCES Team(TeamID),
    Position INT,
    MatchesPlayed INT DEFAULT 0,
    Wins INT DEFAULT 0,
    Draws INT DEFAULT 0,
    Losses INT DEFAULT 0,
    GoalsFor INT DEFAULT 0,
    GoalsAgainst INT DEFAULT 0,
    GoalDifference AS (GoalsFor - GoalsAgainst),
    Points AS (Wins * 3 + Draws * 1),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    UNIQUE(TournamentID, TeamID)
);

-- Bảng Cầu thủ
CREATE TABLE Player (
    PlayerID INT PRIMARY KEY IDENTITY(1,1),
    TeamID INT FOREIGN KEY REFERENCES Team(TeamID),
    PlayerName NVARCHAR(200) NOT NULL,
    DateOfBirth DATE,
    Position NVARCHAR(50),
    JerseyNumber INT,
    Nationality NVARCHAR(100),
    Height DECIMAL(5,2),
    Weight DECIMAL(5,2),
    Photo NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng Thống kê cầu thủ
CREATE TABLE PlayerStatistics (
    StatID INT PRIMARY KEY IDENTITY(1,1),
    PlayerID INT FOREIGN KEY REFERENCES Player(PlayerID),
    TournamentID INT FOREIGN KEY REFERENCES Tournament(TournamentID),
    MatchID INT FOREIGN KEY REFERENCES Match(MatchID),
    Goals INT DEFAULT 0,
    Assists INT DEFAULT 0,
    YellowCards INT DEFAULT 0,
    RedCards INT DEFAULT 0,
    MinutesPlayed INT DEFAULT 0
);

GO

-- =============================================
-- PHẦN 2: TẠO STORED PROCEDURES
-- =============================================

-- SP1: Lấy bảng xếp hạng theo giải đấu
CREATE PROCEDURE sp_GetStandings
    @TournamentID INT = NULL,
    @RoundID INT = NULL
AS
BEGIN
    SELECT 
        s.Position AS [STT],
        t.TeamName AS [Đội Bóng],
        s.MatchesPlayed AS [Số Trận],
        s.Wins AS [Thắng],
        s.Draws AS [Hòa],
        s.Losses AS [Thua],
        s.GoalsFor AS [BT],
        s.GoalsAgainst AS [BB],
        s.GoalDifference AS [HS],
        s.Points AS [Điểm]
    FROM Standings s
    INNER JOIN Team t ON s.TeamID = t.TeamID
    WHERE (@TournamentID IS NULL OR s.TournamentID = @TournamentID)
    ORDER BY s.Points DESC, s.GoalDifference DESC, s.GoalsFor DESC, t.TeamName ASC;
END;
GO

-- SP2: Cập nhật bảng xếp hạng sau khi có kết quả trận đấu
CREATE PROCEDURE sp_UpdateStandingsAfterMatch
    @MatchID INT
AS
BEGIN
    DECLARE @TournamentID INT, @HomeTeamID INT, @AwayTeamID INT;
    DECLARE @HomeScore INT, @AwayScore INT;
    
    SELECT 
        @TournamentID = TournamentID,
        @HomeTeamID = HomeTeamID,
        @AwayTeamID = AwayTeamID,
        @HomeScore = HomeScore,
        @AwayScore = AwayScore
    FROM Match
    WHERE MatchID = @MatchID AND HomeScore IS NOT NULL AND AwayScore IS NOT NULL;
    
    IF @TournamentID IS NULL
        RETURN;
    
    IF NOT EXISTS (SELECT 1 FROM Standings WHERE TournamentID = @TournamentID AND TeamID = @HomeTeamID)
    BEGIN
        INSERT INTO Standings (TournamentID, TeamID, Position)
        VALUES (@TournamentID, @HomeTeamID, 0);
    END
    
    UPDATE Standings
    SET 
        MatchesPlayed = MatchesPlayed + 1,
        Wins = Wins + CASE WHEN @HomeScore > @AwayScore THEN 1 ELSE 0 END,
        Draws = Draws + CASE WHEN @HomeScore = @AwayScore THEN 1 ELSE 0 END,
        Losses = Losses + CASE WHEN @HomeScore < @AwayScore THEN 1 ELSE 0 END,
        GoalsFor = GoalsFor + @HomeScore,
        GoalsAgainst = GoalsAgainst + @AwayScore,
        UpdatedDate = GETDATE()
    WHERE TournamentID = @TournamentID AND TeamID = @HomeTeamID;
    
    IF NOT EXISTS (SELECT 1 FROM Standings WHERE TournamentID = @TournamentID AND TeamID = @AwayTeamID)
    BEGIN
        INSERT INTO Standings (TournamentID, TeamID, Position)
        VALUES (@TournamentID, @AwayTeamID, 0);
    END
    
    UPDATE Standings
    SET 
        MatchesPlayed = MatchesPlayed + 1,
        Wins = Wins + CASE WHEN @AwayScore > @HomeScore THEN 1 ELSE 0 END,
        Draws = Draws + CASE WHEN @AwayScore = @HomeScore THEN 1 ELSE 0 END,
        Losses = Losses + CASE WHEN @AwayScore < @HomeScore THEN 1 ELSE 0 END,
        GoalsFor = GoalsFor + @AwayScore,
        GoalsAgainst = GoalsAgainst + @HomeScore,
        UpdatedDate = GETDATE()
    WHERE TournamentID = @TournamentID AND TeamID = @AwayTeamID;
    
    EXEC sp_RecalculateStandings @TournamentID;
END;
GO

-- SP3: Tính lại bảng xếp hạng
CREATE PROCEDURE sp_RecalculateStandings
    @TournamentID INT
AS
BEGIN
    WITH RankedStandings AS (
        SELECT 
            StandingID,
            ROW_NUMBER() OVER (
                ORDER BY 
                    Points DESC, 
                    GoalDifference DESC, 
                    GoalsFor DESC, 
                    TeamID ASC
            ) AS NewPosition
        FROM Standings
        WHERE TournamentID = @TournamentID
    )
    UPDATE s
    SET s.Position = rs.NewPosition,
        s.UpdatedDate = GETDATE()
    FROM Standings s
    INNER JOIN RankedStandings rs ON s.StandingID = rs.StandingID;
END;
GO

-- SP4: Tìm kiếm đội bóng trong bảng xếp hạng
CREATE PROCEDURE sp_SearchTeamInStandings
    @TournamentID INT,
    @SearchText NVARCHAR(200)
AS
BEGIN
    SELECT 
        s.Position AS [STT],
        t.TeamName AS [Đội Bóng],
        s.MatchesPlayed AS [Số Trận],
        s.Wins AS [Thắng],
        s.Draws AS [Hòa],
        s.Losses AS [Thua],
        s.GoalsFor AS [BT],
        s.GoalsAgainst AS [BB],
        s.GoalDifference AS [HS],
        s.Points AS [Điểm]
    FROM Standings s
    INNER JOIN Team t ON s.TeamID = t.TeamID
    WHERE s.TournamentID = @TournamentID 
        AND t.TeamName LIKE N'%' + @SearchText + '%'
    ORDER BY s.Points DESC, s.GoalDifference DESC, s.GoalsFor DESC, t.TeamName ASC;
END;
GO

-- SP5: Lấy danh sách giải đấu
CREATE PROCEDURE sp_GetTournaments
AS
BEGIN
    SELECT 
        TournamentID,
        TournamentName,
        Season,
        StartDate,
        EndDate,
        Status
    FROM Tournament
    ORDER BY StartDate DESC;
END;
GO

-- SP6: Lấy danh sách vòng đấu theo giải
CREATE PROCEDURE sp_GetRoundsByTournament
    @TournamentID INT
AS
BEGIN
    SELECT 
        RoundID,
        RoundNumber,
        RoundName,
        StartDate,
        EndDate,
        Status
    FROM Round
    WHERE TournamentID = @TournamentID
    ORDER BY RoundNumber ASC;
END;
GO

-- SP7: Reset bảng xếp hạng
CREATE PROCEDURE sp_ResetStandings
    @TournamentID INT
AS
BEGIN
    UPDATE Standings
    SET 
        MatchesPlayed = 0,
        Wins = 0,
        Draws = 0,
        Losses = 0,
        GoalsFor = 0,
        GoalsAgainst = 0,
        Position = 0,
        UpdatedDate = GETDATE()
    WHERE TournamentID = @TournamentID;
END;
GO

-- =============================================
-- PHẦN 3: DỮ LIỆU MẪU
-- =============================================

-- Insert Giải đấu
INSERT INTO Tournament (TournamentName, Season, StartDate, EndDate, Status)
VALUES 
    (N'Giải VĐQG 2026', N'2026', '2026-01-01', '2026-12-31', N'Đang diễn ra'),
    (N'Cup Sinh viên 2026', N'2026', '2026-03-01', '2026-06-30', N'Đang diễn ra');

-- Insert Đội bóng
INSERT INTO Team (TeamName, ShortName, Stadium, Coach)
VALUES 
    (N'Hà Nội FC', N'HN', N'Sân Hàng Đẫy', N'Nguyễn Văn A'),
    (N'Hoàng Anh Gia Lai', N'HAGL', N'Sân Pleiku', N'Trần Văn B'),
    (N'Sài Gòn FC', N'SG', N'Sân Thống Nhất', N'Lê Văn C'),
    (N'Viettel FC', N'VT', N'Sân Hàng Đẫy', N'Phạm Văn D'),
    (N'Thanh Hóa FC', N'TH', N'Sân Thanh Hóa', N'Hoàng Văn E');

-- Insert Vòng đấu
INSERT INTO Round (TournamentID, RoundNumber, RoundName, Status)
VALUES 
    (1, 1, N'Vòng 1', N'Hoàn thành'),
    (1, 2, N'Vòng 2', N'Hoàn thành'),
    (1, 3, N'Vòng 3', N'Đang diễn ra');

-- Insert Bảng xếp hạng mẫu
INSERT INTO Standings (TournamentID, TeamID, Position, MatchesPlayed, Wins, Draws, Losses, GoalsFor, GoalsAgainst)
VALUES 
    (1, 1, 1, 2, 2, 0, 0, 5, 1),
    (1, 2, 2, 2, 1, 1, 0, 4, 2),
    (1, 3, 3, 2, 1, 0, 1, 3, 3),
    (1, 4, 4, 2, 0, 1, 1, 2, 4),
    (1, 5, 5, 2, 0, 0, 2, 1, 5);

GO

-- =============================================
-- HOÀN TẤT
-- =============================================
PRINT 'Database FootballManagementDB đã được tạo thành công!';
PRINT 'Bao gồm: 7 bảng, 7 stored procedures, và dữ liệu mẫu';
GO
