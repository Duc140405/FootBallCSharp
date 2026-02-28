using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("CoachHistory")]
    public class CoachHistory
    {
        [Key]
        public int HistoryID { get; set; }

        public int CoachID { get; set; }
        public int TeamID { get; set; }
        public int FromYear { get; set; }
        public int? ToYear { get; set; }

        [StringLength(255)]
        public string Achievement { get; set; }

        [ForeignKey("CoachID")]
        public virtual Coach Coach { get; set; }

        [ForeignKey("TeamID")]
        public virtual Team Team { get; set; }
    }
}
