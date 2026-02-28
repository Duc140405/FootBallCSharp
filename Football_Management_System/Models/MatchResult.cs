using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("MatchResults")]
    public class MatchResult
    {
        [Key]
        public int ResultID { get; set; }

        [ForeignKey("Match")]
        public int MatchID { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public int? HomeYellowCards { get; set; }
        public int? AwayYellowCards { get; set; }
        public int? HomeRedCards { get; set; }
        public int? AwayRedCards { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        public virtual Match Match { get; set; }
    }
}
