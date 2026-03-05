using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("Rounds")]
    public class Round
    {
        [Key]
        public int RoundID { get; set; }

        public int TournamentID { get; set; }
        public int RoundNumber { get; set; }

        [StringLength(100)]
        public string RoundName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("TournamentID")]
        public virtual Tournament Tournament { get; set; }

        public virtual ICollection<Match> Matches { get; set; }

        public Round()
        {
            Matches = new HashSet<Match>();
        }
    }
}
