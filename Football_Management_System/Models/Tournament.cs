using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("Tournaments")]
    public class Tournament
    {
        [Key]
        public int TournamentID { get; set; }

        [Required]
        [StringLength(200)]
        public string TournamentName { get; set; }

        public int? TotalRounds { get; set; }

        [StringLength(50)]
        public string Season { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Round> Rounds { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
        public virtual ICollection<Standing> Standings { get; set; }
        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; }

        public Tournament()
        {
            Teams = new HashSet<Team>();
            Rounds = new HashSet<Round>();
            Matches = new HashSet<Match>();
            Standings = new HashSet<Standing>();
            PlayerStatistics = new HashSet<PlayerStatistic>();
        }
    }
}
