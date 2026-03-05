CREATE TABLE teams (
    team_id INT IDENTITY(1,1) PRIMARY KEY,
    team_name NVARCHAR(150) NOT NULL UNIQUE,
    logo_path NVARCHAR(255),
    tournament_id INT,
    coach_id INT,
    status NVARCHAR(50)
        CHECK (status IN (N'Đang thi đấu', N'Tạm dừng')),
    FOREIGN KEY (tournament_id)
        REFERENCES tournaments(tournament_id)
        ON DELETE SET NULL,
    FOREIGN KEY (coach_id)
        REFERENCES coaches(coach_id)
        ON DELETE SET NULL
);
CREATE TABLE Teams (
TeamId INT PRIMARY KEY IDENTITY,
TeamName NVARCHAR(100) NOT NULL
);

CREATE TABLE Matches (
MatchId INT PRIMARY KEY IDENTITY,
HomeTeam NVARCHAR(100),
AwayTeam NVARCHAR(100),
MatchDate DATETIME,
Stadium NVARCHAR(100),
Referee NVARCHAR(100)
);

CREATE TABLE MatchResults (
ResultId INT PRIMARY KEY IDENTITY,
MatchId INT FOREIGN KEY REFERENCES Matches(MatchId),
HomeScore INT,
AwayScore INT,
Status NVARCHAR(50)
);
CREATE TABLE players (
    player_id INT IDENTITY(1,1) PRIMARY KEY,

    team_id INT NOT NULL,

    full_name NVARCHAR(150) NOT NULL,
    date_of_birth DATE NULL,
    nationality NVARCHAR(80) NULL,

    shirt_number INT,
    position NVARCHAR(10)
        CHECK (position IN ('GK','DF','MF','FW')),

    sub_position NVARCHAR(50) NULL,

    preferred_foot NVARCHAR(20)
        CHECK (preferred_foot IN (N'Phải', N'Trái', N'Cả hai')),

    height_cm INT NULL,
    weight_kg INT NULL,

    status NVARCHAR(30)
        CHECK (status IN (N'Sẵn sàng', N'Chấn thương', N'Treo giò')),

    technical_score INT
        CHECK (technical_score BETWEEN 1 AND 10),

    avatar_path NVARCHAR(255) NULL,

    created_at DATETIME DEFAULT GETDATE(),

    -- Ràng buộc
    CONSTRAINT FK_players_teams
        FOREIGN KEY (team_id)
        REFERENCES teams(team_id)
        ON DELETE CASCADE,

    CONSTRAINT UQ_team_shirt
        UNIQUE (team_id, shirt_number)
);
CREATE TABLE player_general_statistics (
    player_id INT PRIMARY KEY,

    matches INT DEFAULT 0,
    goals INT DEFAULT 0,
    assists INT DEFAULT 0,
    yellow_cards INT DEFAULT 0,
    red_cards INT DEFAULT 0,

    CONSTRAINT FK_player_general_statistics
        FOREIGN KEY (player_id)
        REFERENCES players(player_id)
        ON DELETE CASCADE
);
CREATE TABLE player_notes (
    note_id INT IDENTITY(1,1) PRIMARY KEY,
    player_id INT NOT NULL,
    note_content NVARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_player_notes
        FOREIGN KEY (player_id)
        REFERENCES players(player_id)
        ON DELETE CASCADE
);
CREATE TABLE player_attachments (
    attachment_id INT IDENTITY(1,1) PRIMARY KEY,
    player_id INT NOT NULL,
    file_name NVARCHAR(255),
    file_path NVARCHAR(255),
    uploaded_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_player_attachments
        FOREIGN KEY (player_id)
        REFERENCES players(player_id)
        ON DELETE CASCADE
);
SELECT
    p.player_id,
    p.full_name       AS [Họ Tên],
    p.shirt_number    AS [Số Áo],
    p.position        AS [Vị Trí],
    p.nationality     AS [Quốc Tịch],
    p.status          AS [Trạng Thái]
FROM players p;
CREATE TABLE coach_history (
    history_id INT IDENTITY(1,1) PRIMARY KEY,
    coach_id INT NOT NULL,
    team_id INT NOT NULL,
    from_year INT NOT NULL,
    to_year INT NULL,
    achievement NVARCHAR(255),

    FOREIGN KEY (coach_id) REFERENCES coaches(coach_id) ON DELETE CASCADE,
    FOREIGN KEY (team_id) REFERENCES teams(team_id) ON DELETE CASCADE
);
CREATE TABLE teams (
    team_id INT IDENTITY(1,1) PRIMARY KEY,
    team_name NVARCHAR(150) NOT NULL
);
CREATE TABLE coaches (
    coach_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(150) NOT NULL,
    birth_date DATE NOT NULL,
    nationality NVARCHAR(100),
    experience_years INT NOT NULL,
    current_team_id INT NULL,

    FOREIGN KEY (current_team_id) REFERENCES teams(team_id)
);
CREATE TABLE Tournaments (
TournamentID INT IDENTITY(1,1) PRIMARY KEY,
TenGiai NVARCHAR(150) NOT NULL, 
SoVong INT NOT NULL DEFAULT 0,
NgayBD DATE NOT NULL,
NgayKT DATE NOT NULL,
CONSTRAINT chk_tournament_dates CHECK (NgayKT >= NgayBD)
);
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    FullName NVARCHAR(100),
    RoleId INT NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,

    CONSTRAINT FK_User_Role FOREIGN KEY (RoleId)
    REFERENCES Roles(RoleId)
);
CREATE DATABASE FootballManagementDB;
GO

USE FootballManagementDB;
GO

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

CREATE TABLE Round (
    RoundID INT PRIMARY KEY IDENTITY(1,1),
    TournamentID INT FOREIGN KEY REFERENCES Tournament(TournamentID),
    RoundNumber INT NOT NULL,
    RoundName NVARCHAR(100),
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT N'Chưa bắt đầu'
);

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

CREATE PROCEDURE sp_GetStandings
    @TournamentID INT = NULL,
    @RoundID INT = NULL
AS
BEGIN
    SELECT s.Position AS [STT], t.TeamName AS [Đội Bóng], s.MatchesPlayed AS [Số Trận],
           s.Wins AS [Thắng], s.Draws AS [Hòa], s.Losses AS [Thua],
           s.GoalsFor AS [BT], s.GoalsAgainst AS [BB], s.GoalDifference AS [HS], s.Points AS [Điểm]
    FROM Standings s INNER JOIN Team t ON s.TeamID = t.TeamID
    WHERE (@TournamentID IS NULL OR s.TournamentID = @TournamentID)
    ORDER BY s.Points DESC, s.GoalDifference DESC, s.GoalsFor DESC, t.TeamName ASC;
END;
GO

CREATE PROCEDURE sp_UpdateStandingsAfterMatch @MatchID INT
AS
BEGIN
    DECLARE @TournamentID INT, @HomeTeamID INT, @AwayTeamID INT, @HomeScore INT, @AwayScore INT;
    SELECT @TournamentID = TournamentID, @HomeTeamID = HomeTeamID, @AwayTeamID = AwayTeamID,
           @HomeScore = HomeScore, @AwayScore = AwayScore
    FROM Match WHERE MatchID = @MatchID AND HomeScore IS NOT NULL AND AwayScore IS NOT NULL;
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
END;
GO

CREATE PROCEDURE sp_RecalculateStandings @TournamentID INT
AS
BEGIN
    WITH RankedStandings AS (
        SELECT StandingID, ROW_NUMBER() OVER (ORDER BY Points DESC, GoalDifference DESC, GoalsFor DESC, TeamID ASC) AS NewPosition
        FROM Standings WHERE TournamentID = @TournamentID
    )
    UPDATE s SET s.Position = rs.NewPosition, s.UpdatedDate = GETDATE()
    FROM Standings s INNER JOIN RankedStandings rs ON s.StandingID = rs.StandingID;
END;
GO

CREATE PROCEDURE sp_SearchTeamInStandings @TournamentID INT, @SearchText NVARCHAR(200)
AS
BEGIN
    SELECT s.Position AS [STT], t.TeamName AS [Đội Bóng], s.MatchesPlayed AS [Số Trận],
           s.Wins AS [Thắng], s.Draws AS [Hòa], s.Losses AS [Thua],
           s.GoalsFor AS [BT], s.GoalsAgainst AS [BB], s.GoalDifference AS [HS], s.Points AS [Điểm]
    FROM Standings s INNER JOIN Team t ON s.TeamID = t.TeamID
    WHERE s.TournamentID = @TournamentID AND t.TeamName LIKE N'%' + @SearchText + '%'
    ORDER BY s.Points DESC, s.GoalDifference DESC, s.GoalsFor DESC, t.TeamName ASC;
END;
GO

CREATE PROCEDURE sp_GetTournaments
AS
BEGIN
    SELECT TournamentID, TournamentName, Season, StartDate, EndDate, Status
    FROM Tournament ORDER BY StartDate DESC;
END;
GO

CREATE PROCEDURE sp_GetRoundsByTournament @TournamentID INT
AS
BEGIN
    SELECT RoundID, RoundNumber, RoundName, StartDate, EndDate, Status
    FROM Round WHERE TournamentID = @TournamentID ORDER BY RoundNumber ASC;
END;
GO

CREATE PROCEDURE sp_ResetStandings @TournamentID INT
AS
BEGIN
    UPDATE Standings SET MatchesPlayed = 0, Wins = 0, Draws = 0, Losses = 0,
        GoalsFor = 0, GoalsAgainst = 0, Position = 0, UpdatedDate = GETDATE()
    WHERE TournamentID = @TournamentID;
END;
GO

INSERT INTO Tournament (TournamentName, Season, StartDate, EndDate, Status) VALUES 
    (N'Giải VĐQG 2026', N'2026', '2026-01-01', '2026-12-31', N'Đang diễn ra'),
    (N'Cup Sinh viên 2026', N'2026', '2026-03-01', '2026-06-30', N'Đang diễn ra');

INSERT INTO Team (TeamName, ShortName, Stadium, Coach) VALUES 
    (N'Hà Nội FC', N'HN', N'Sân Hàng Đẫy', N'Nguyễn Văn A'),
    (N'Hoàng Anh Gia Lai', N'HAGL', N'Sân Pleiku', N'Trần Văn B'),
    (N'Sài Gòn FC', N'SG', N'Sân Thống Nhất', N'Lê Văn C'),
    (N'Viettel FC', N'VT', N'Sân Hàng Đẫy', N'Phạm Văn D'),
    (N'Thanh Hóa FC', N'TH', N'Sân Thanh Hóa', N'Hoàng Văn E');

INSERT INTO Round (TournamentID, RoundNumber, RoundName, Status) VALUES 
    (1, 1, N'Vòng 1', N'Hoàn thành'),
    (1, 2, N'Vòng 2', N'Hoàn thành'),
    (1, 3, N'Vòng 3', N'Đang diễn ra');

INSERT INTO Standings (TournamentID, TeamID, Position, MatchesPlayed, Wins, Draws, Losses, GoalsFor, GoalsAgainst) VALUES 
    (1, 1, 1, 2, 2, 0, 0, 5, 1),
    (1, 2, 2, 2, 1, 1, 0, 4, 2),
    (1, 3, 3, 2, 1, 0, 1, 3, 3),
    (1, 4, 4, 2, 0, 1, 1, 2, 4),
    (1, 5, 5, 2, 0, 0, 2, 1, 5);
GO

-- ============================================
-- PHẦN: Match Result Window (Nguyễn Tấn Đức)
-- ============================================
CREATE TABLE Teams (
    TeamId INT IDENTITY(1,1) PRIMARY KEY,
    TeamName NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Matches (
    MatchId INT IDENTITY(1,1) PRIMARY KEY,
    HomeTeamId INT NOT NULL,
    AwayTeamId INT NOT NULL,
    MatchDate DATETIME NOT NULL,

    CONSTRAINT FK_Matches_HomeTeam FOREIGN KEY (HomeTeamId) REFERENCES Teams(TeamId),
    CONSTRAINT FK_Matches_AwayTeam FOREIGN KEY (AwayTeamId) REFERENCES Teams(TeamId),
    CONSTRAINT CHK_DifferentTeams CHECK (HomeTeamId != AwayTeamId)
);
GO

CREATE TABLE MatchResults (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    MatchId INT NOT NULL UNIQUE,
    HomeScore INT DEFAULT 0,
    AwayScore INT DEFAULT 0,
    HomeYellowCards INT DEFAULT 0,
    AwayYellowCards INT DEFAULT 0,
    HomeRedCards INT DEFAULT 0,
    AwayRedCards INT DEFAULT 0,
    Note NVARCHAR(500),

    CONSTRAINT FK_MatchResults_Matches FOREIGN KEY (MatchId) REFERENCES Matches(MatchId) ON DELETE CASCADE,
    CONSTRAINT CHK_HomeScore CHECK (HomeScore >= 0),
    CONSTRAINT CHK_AwayScore CHECK (AwayScore >= 0),
    CONSTRAINT CHK_HomeYellow CHECK (HomeYellowCards >= 0),
    CONSTRAINT CHK_AwayYellow CHECK (AwayYellowCards >= 0),
    CONSTRAINT CHK_HomeRed CHECK (HomeRedCards >= 0),
    CONSTRAINT CHK_AwayRed CHECK (AwayRedCards >= 0)
);
GO

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

    SELECT @HomeTeamId = TeamId FROM Teams WHERE TeamName = @HomeTeam;
    IF @HomeTeamId IS NULL
    BEGIN
        INSERT INTO Teams (TeamName) VALUES (@HomeTeam);
        SET @HomeTeamId = SCOPE_IDENTITY();
    END

    SELECT @AwayTeamId = TeamId FROM Teams WHERE TeamName = @AwayTeam;
    IF @AwayTeamId IS NULL
    BEGIN
        INSERT INTO Teams (TeamName) VALUES (@AwayTeam);
        SET @AwayTeamId = SCOPE_IDENTITY();
    END

    INSERT INTO Matches (HomeTeamId, AwayTeamId, MatchDate)
    VALUES (@HomeTeamId, @AwayTeamId, @MatchDate);
    SET @MatchId = SCOPE_IDENTITY();

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

CREATE PROCEDURE sp_DeleteMatch
    @MatchId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Matches WHERE MatchId = @MatchId;
END
GO

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

INSERT INTO Teams (TeamName) VALUES
(N'Manchester United'),
(N'Manchester City'),
(N'Liverpool'),
(N'Chelsea'),
(N'Arsenal'),
(N'Tottenham');
GO

INSERT INTO Matches (HomeTeamId, AwayTeamId, MatchDate) VALUES
(1, 2, '2026-03-01 19:00:00'),
(3, 4, '2026-03-02 21:00:00'),
(5, 6, '2026-03-03 18:30:00');
GO

INSERT INTO MatchResults (MatchId, HomeScore, AwayScore, HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards, Note)
VALUES (1, 2, 1, 3, 2, 0, 1, N'Trận derby Manchester');
GO
