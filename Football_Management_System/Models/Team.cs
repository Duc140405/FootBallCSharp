using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("Teams")]
    public class Team
    {
        [Key]
        public int TeamID { get; set; }

        [Required]
        [StringLength(200)]
        public string TeamName { get; set; }

        [StringLength(50)]
        public string ShortName { get; set; }

        [StringLength(255)]
        public string LogoPath { get; set; }

        [StringLength(200)]
        public string Stadium { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int? TournamentID { get; set; }
        public int? CoachID { get; set; }
        public int? FoundedYear { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("TournamentID")]
        public virtual Tournament Tournament { get; set; }

        [ForeignKey("CoachID")]
        public virtual Coach Coach { get; set; }

        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Standing> Standings { get; set; }

        [InverseProperty("HomeTeam")]
        public virtual ICollection<Match> HomeMatches { get; set; }

        [InverseProperty("AwayTeam")]
        public virtual ICollection<Match> AwayMatches { get; set; }

        public Team()
        {
            Players = new HashSet<Player>();
            Standings = new HashSet<Standing>();
            HomeMatches = new HashSet<Match>();
            AwayMatches = new HashSet<Match>();
        }
    }
}
