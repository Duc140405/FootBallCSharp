USE FootballManagementDB;
GO

-- Stored Procedure: Lấy bảng xếp hạng theo giải đấu
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

-- Stored Procedure: Cập nhật bảng xếp hạng sau khi có kết quả trận đấu
CREATE PROCEDURE sp_UpdateStandingsAfterMatch
    @MatchID INT
AS
BEGIN
    DECLARE @TournamentID INT, @HomeTeamID INT, @AwayTeamID INT;
    DECLARE @HomeScore INT, @AwayScore INT;
    
    -- Lấy thông tin trận đấu
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
    
    -- Cập nhật cho đội nhà
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
    
    -- Cập nhật cho đội khách
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
    
    -- Cập nhật lại thứ hạng
    EXEC sp_RecalculateStandings @TournamentID;
END;
GO

-- Stored Procedure: Tính lại bảng xếp hạng
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

-- Stored Procedure: Tìm kiếm đội bóng trong bảng xếp hạng
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

-- Stored Procedure: Lấy danh sách giải đấu
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

-- Stored Procedure: Lấy danh sách vòng đấu theo giải
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

-- Stored Procedure: Reset bảng xếp hạng
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
