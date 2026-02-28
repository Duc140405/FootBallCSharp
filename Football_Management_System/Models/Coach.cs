using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("Coaches")]
    public class Coach
    {
        [Key]
        public int CoachID { get; set; }

        [Required]
        [StringLength(150)]
        public string CoachName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [StringLength(100)]
        public string Nationality { get; set; }

        public int? ExperienceYears { get; set; }
        public int? CurrentTeamID { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<CoachHistory> CoachHistories { get; set; }

        public Coach()
        {
            Teams = new HashSet<Team>();
            CoachHistories = new HashSet<CoachHistory>();
        }
    }
}
