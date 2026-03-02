using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Football_Management_System.Models
{
    [Table("Matches")]
    public class Match
    {
        [Key]
        public int MatchID { get; set; }

        public int? TournamentID { get; set; }
        public int? RoundID { get; set; }
        public int HomeTeamID { get; set; }
        public int AwayTeamID { get; set; }
        public DateTime MatchDate { get; set; }

        [StringLength(200)]
        public string Stadium { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("TournamentID")]
        public virtual Tournament Tournament { get; set; }

        [ForeignKey("RoundID")]
        public virtual Round Round { get; set; }

        [ForeignKey("HomeTeamID")]
        public virtual Team HomeTeam { get; set; }

        [ForeignKey("AwayTeamID")]
        public virtual Team AwayTeam { get; set; }

        public virtual ICollection<MatchResult> MatchResultItems { get; set; }
        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; }

        [NotMapped]
        public MatchResult MatchResult
        {
            get { return MatchResultItems != null ? MatchResultItems.FirstOrDefault() : null; }
        }

        [NotMapped]
        public string HomeTeamName
        {
            get { return HomeTeam != null ? HomeTeam.TeamName : ""; }
        }

        [NotMapped]
        public string AwayTeamName
        {
            get { return AwayTeam != null ? AwayTeam.TeamName : ""; }
        }

        [NotMapped]
        public string DisplayName
        {
            get { return string.Format("{0} vs {1} ({2:dd/MM/yyyy})", HomeTeamName, AwayTeamName, MatchDate); }
        }

        [NotMapped]
        public string Result
        {
            get
            {
                if (MatchResult != null && MatchResult.HomeScore.HasValue)
                    return string.Format("{0} - {1}", MatchResult.HomeScore, MatchResult.AwayScore);
                return "Chưa có";
            }
        }

        [NotMapped]
        public string TotalYellow
        {
            get
            {
                if (MatchResult == null) return "-";
                int total = (MatchResult.HomeYellowCards ?? 0) + (MatchResult.AwayYellowCards ?? 0);
                return total > 0 ? total.ToString() : "-";
            }
        }

        [NotMapped]
        public string TotalRed
        {
            get
            {
                if (MatchResult == null) return "-";
                int total = (MatchResult.HomeRedCards ?? 0) + (MatchResult.AwayRedCards ?? 0);
                return total > 0 ? total.ToString() : "-";
            }
        }

        [NotMapped]
        public string MatchStatus
        {
            get
            {
                if (MatchResult != null && MatchResult.HomeScore.HasValue)
                    return "✅ Hoàn thành";
                return "⏳ Chờ KQ";
            }
        }

        public Match()
        {
            PlayerStatistics = new HashSet<PlayerStatistic>();
            MatchResultItems = new HashSet<MatchResult>();
        }
    }
}
