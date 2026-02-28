using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("Players")]
    public class Player
    {
        [Key]
        public int PlayerID { get; set; }

        public int TeamID { get; set; }

        [Required]
        [StringLength(200)]
        public string PlayerName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string Nationality { get; set; }

        public int? JerseyNumber { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(50)]
        public string SubPosition { get; set; }

        [StringLength(20)]
        public string PreferredFoot { get; set; }

        public int? HeightCm { get; set; }
        public int? WeightKg { get; set; }

        [StringLength(30)]
        public string Status { get; set; }

        public int? TechnicalScore { get; set; }

        [StringLength(500)]
        public string Photo { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("TeamID")]
        public virtual Team Team { get; set; }

        public virtual PlayerGeneralStatistic GeneralStatistic { get; set; }
        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; }
        public virtual ICollection<PlayerNote> PlayerNotes { get; set; }
        public virtual ICollection<PlayerAttachment> PlayerAttachments { get; set; }

        public Player()
        {
            PlayerStatistics = new HashSet<PlayerStatistic>();
            PlayerNotes = new HashSet<PlayerNote>();
            PlayerAttachments = new HashSet<PlayerAttachment>();
        }
    }
}
