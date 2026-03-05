using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("PlayerStatistics")]
    public class PlayerStatistic
    {
        [Key]
        public int StatID { get; set; }

        public int PlayerID { get; set; }
        public int TournamentID { get; set; }
        public int MatchID { get; set; }
        public int? Goals { get; set; }
        public int? Assists { get; set; }
        public int? YellowCards { get; set; }
        public int? RedCards { get; set; }
        public int? MinutesPlayed { get; set; }

        [ForeignKey("PlayerID")]
        public virtual Player Player { get; set; }

        [ForeignKey("TournamentID")]
        public virtual Tournament Tournament { get; set; }

        [ForeignKey("MatchID")]
        public virtual Match Match { get; set; }
    }

    [Table("PlayerGeneralStatistics")]
    public class PlayerGeneralStatistic
    {
        [Key]
        [ForeignKey("Player")]
        public int PlayerID { get; set; }

        public int? Matches { get; set; }
        public int? Goals { get; set; }
        public int? Assists { get; set; }
        public int? YellowCards { get; set; }
        public int? RedCards { get; set; }

        public virtual Player Player { get; set; }
    }

    [Table("PlayerNotes")]
    public class PlayerNote
    {
        [Key]
        public int NoteID { get; set; }

        public int PlayerID { get; set; }
        public string NoteContent { get; set; }
        public DateTime? CreatedDate { get; set; }

        [ForeignKey("PlayerID")]
        public virtual Player Player { get; set; }
    }

    [Table("PlayerAttachments")]
    public class PlayerAttachment
    {
        [Key]
        public int AttachmentID { get; set; }

        public int PlayerID { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(255)]
        public string FilePath { get; set; }

        public DateTime? UploadedDate { get; set; }

        [ForeignKey("PlayerID")]
        public virtual Player Player { get; set; }
    }
}
