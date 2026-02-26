-- Football Management System Database
-- Quản lý bảng xếp hạng giải đấu

-- Tạo database
CREATE DATABASE FootballManagementDB;
GO

USE FootballManagementDB;
GO

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

-- Insert dữ liệu mẫu
INSERT INTO Tournament (TournamentName, Season, StartDate, EndDate, Status)
VALUES 
    (N'Giải VĐQG 2026', N'2026', '2026-01-01', '2026-12-31', N'Đang diễn ra'),
    (N'Cup Sinh viên 2026', N'2026', '2026-03-01', '2026-06-30', N'Đang diễn ra');

INSERT INTO Team (TeamName, ShortName, Stadium, Coach)
VALUES 
    (N'Hà Nội FC', N'HN', N'Sân Hàng Đẫy', N'Nguyễn Văn A'),
    (N'Hoàng Anh Gia Lai', N'HAGL', N'Sân Pleiku', N'Trần Văn B'),
    (N'Sài Gòn FC', N'SG', N'Sân Thống Nhất', N'Lê Văn C'),
    (N'Viettel FC', N'VT', N'Sân Hàng Đẫy', N'Phạm Văn D'),
    (N'Thanh Hóa FC', N'TH', N'Sân Thanh Hóa', N'Hoàng Văn E');

INSERT INTO Round (TournamentID, RoundNumber, RoundName, Status)
VALUES 
    (1, 1, N'Vòng 1', N'Hoàn thành'),
    (1, 2, N'Vòng 2', N'Hoàn thành'),
    (1, 3, N'Vòng 3', N'Đang diễn ra');

-- Insert dữ liệu mẫu cho bảng xếp hạng
INSERT INTO Standings (TournamentID, TeamID, Position, MatchesPlayed, Wins, Draws, Losses, GoalsFor, GoalsAgainst)
VALUES 
    (1, 1, 1, 2, 2, 0, 0, 5, 1),
    (1, 2, 2, 2, 1, 1, 0, 4, 2),
    (1, 3, 3, 2, 1, 0, 1, 3, 3),
    (1, 4, 4, 2, 0, 1, 1, 2, 4),
    (1, 5, 5, 2, 0, 0, 2, 1, 5);

GO
