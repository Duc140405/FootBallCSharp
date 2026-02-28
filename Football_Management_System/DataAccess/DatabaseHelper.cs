using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Football_Management_System.DataAccess
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["FootballDB"].ConnectionString;
        }

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region Match Operations

        /// <summary>
        /// Lấy tất cả trận đấu với kết quả
        /// </summary>
        public List<Match> GetAllMatches()
        {
            var matches = new List<Match>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM vw_MatchDetails ORDER BY MatchDate DESC";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matches.Add(MapToMatch(reader));
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Thêm trận đấu mới
        /// </summary>
        public int AddMatch(Match match)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("sp_AddMatch", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@HomeTeam", match.HomeTeam);
                    cmd.Parameters.AddWithValue("@AwayTeam", match.AwayTeam);
                    cmd.Parameters.AddWithValue("@MatchDate", match.MatchDate);
                    cmd.Parameters.AddWithValue("@HomeScore", (object)match.HomeScore ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AwayScore", (object)match.AwayScore ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HomeYellowCards", (object)match.HomeYellowCards ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AwayYellowCards", (object)match.AwayYellowCards ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HomeRedCards", (object)match.HomeRedCards ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AwayRedCards", (object)match.AwayRedCards ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Note", (object)match.Note ?? DBNull.Value);

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        /// <summary>
        /// Cập nhật kết quả trận đấu
        /// </summary>
        public bool UpdateMatchResult(Match match)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("sp_UpdateMatchResult", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MatchId", match.MatchId);
                    cmd.Parameters.AddWithValue("@HomeScore", match.HomeScore ?? 0);
                    cmd.Parameters.AddWithValue("@AwayScore", match.AwayScore ?? 0);
                    cmd.Parameters.AddWithValue("@HomeYellowCards", match.HomeYellowCards ?? 0);
                    cmd.Parameters.AddWithValue("@AwayYellowCards", match.AwayYellowCards ?? 0);
                    cmd.Parameters.AddWithValue("@HomeRedCards", match.HomeRedCards ?? 0);
                    cmd.Parameters.AddWithValue("@AwayRedCards", match.AwayRedCards ?? 0);
                    cmd.Parameters.AddWithValue("@Note", (object)match.Note ?? DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Xóa trận đấu
        /// </summary>
        public bool DeleteMatch(int matchId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("sp_DeleteMatch", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MatchId", matchId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Tìm kiếm trận đấu theo tên đội
        /// </summary>
        public List<Match> SearchMatches(string keyword)
        {
            var matches = new List<Match>();

            using (var conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("sp_SearchMatches", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Keyword", keyword);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            matches.Add(MapToMatch(reader));
                        }
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Lấy thống kê
        /// </summary>
        public MatchStatistics GetStatistics()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("sp_GetStatistics", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MatchStatistics
                            {
                                TongTran = reader.GetInt32(reader.GetOrdinal("TongTran")),
                                DaCoKetQua = reader.GetInt32(reader.GetOrdinal("DaCoKetQua")),
                                ChuaCoKetQua = reader.GetInt32(reader.GetOrdinal("ChuaCoKetQua"))
                            };
                        }
                    }
                }
            }

            return new MatchStatistics();
        }

        #endregion

        #region Team / Player Operations

        /// <summary>
        /// Lấy số lượng đội
        /// </summary>
        public int GetTeamsCount()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Teams", conn))
                {
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        /// <summary>
        /// Lấy số lượng cầu thủ
        /// </summary>
        public int GetPlayersCount()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Players", conn))
                {
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        /// <summary>
        /// Lấy thống kê theo đội: tên đội, số cầu thủ, số trận
        /// </summary>
        public List<TeamStat> GetTeamStats()
        {
            var list = new List<TeamStat>();

            using (var conn = GetConnection())
            {
                conn.Open();

                string query = @"
SELECT t.TeamName AS Team,
       ISNULL(p.PlayerCount, 0) AS Players,
       ISNULL(m.MatchCount, 0) AS Matches
FROM Teams t
LEFT JOIN (
    SELECT TeamId, COUNT(*) AS PlayerCount FROM Players GROUP BY TeamId
) p ON p.TeamId = t.TeamId
LEFT JOIN (
    SELECT TeamId, SUM(CountMatches) AS MatchCount FROM (
        SELECT HomeTeamId AS TeamId, COUNT(*) AS CountMatches FROM Matches GROUP BY HomeTeamId
        UNION ALL
        SELECT AwayTeamId AS TeamId, COUNT(*) AS CountMatches FROM Matches GROUP BY AwayTeamId
    ) x GROUP BY TeamId
) m ON m.TeamId = t.TeamId
ORDER BY t.TeamName";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TeamStat
                        {
                            Team = reader.IsDBNull(reader.GetOrdinal("Team")) ? string.Empty : reader.GetString(reader.GetOrdinal("Team")),
                            Players = reader.IsDBNull(reader.GetOrdinal("Players")) ? 0 : reader.GetInt32(reader.GetOrdinal("Players")),
                            Matches = reader.IsDBNull(reader.GetOrdinal("Matches")) ? 0 : reader.GetInt32(reader.GetOrdinal("Matches"))
                        });
                    }
                }
            }

            return list;
        }

        #endregion

        #region Helper Methods

        private Match MapToMatch(SqlDataReader reader)
        {
            return new Match
            {
                MatchId = reader.GetInt32(reader.GetOrdinal("MatchId")),
                HomeTeam = reader.GetString(reader.GetOrdinal("HomeTeam")),
                AwayTeam = reader.GetString(reader.GetOrdinal("AwayTeam")),
                MatchDate = reader.GetDateTime(reader.GetOrdinal("MatchDate")),
                HomeScore = reader.IsDBNull(reader.GetOrdinal("HomeScore")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("HomeScore")),
                AwayScore = reader.IsDBNull(reader.GetOrdinal("AwayScore")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("AwayScore")),
                HomeYellowCards = reader.IsDBNull(reader.GetOrdinal("HomeYellowCards")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("HomeYellowCards")),
                AwayYellowCards = reader.IsDBNull(reader.GetOrdinal("AwayYellowCards")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("AwayYellowCards")),
                HomeRedCards = reader.IsDBNull(reader.GetOrdinal("HomeRedCards")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("HomeRedCards")),
                AwayRedCards = reader.IsDBNull(reader.GetOrdinal("AwayRedCards")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("AwayRedCards")),
                Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note"))
            };
        }

        /// <summary>
        /// Kiểm tra kết nối database
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// Lớp chứa thông tin thống kê trận đấu
    /// </summary>
    public class MatchStatistics
    {
        public int TongTran { get; set; }
        public int DaCoKetQua { get; set; }
        public int ChuaCoKetQua { get; set; }
    }

    /// <summary>
    /// Lớp chứa thống kê theo đội
    /// </summary>
    public class TeamStat
    {
        public string Team { get; set; }
        public int Players { get; set; }
        public int Matches { get; set; }
    }
}
