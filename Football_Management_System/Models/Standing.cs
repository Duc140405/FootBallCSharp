using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("Standings")]
    public class Standing
    {
        [Key]
        public int StandingID { get; set; }

        public int TournamentID { get; set; }
        public int TeamID { get; set; }
        public int? Position { get; set; }
        public int? MatchesPlayed { get; set; }
        public int? Wins { get; set; }
        public int? Draws { get; set; }
        public int? Losses { get; set; }
        public int? GoalsFor { get; set; }
        public int? GoalsAgainst { get; set; }
        public int? GoalDifference { get; set; }
        public int? Points { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("TournamentID")]
        public virtual Tournament Tournament { get; set; }

        [ForeignKey("TeamID")]
        public virtual Team Team { get; set; }
    }
}
