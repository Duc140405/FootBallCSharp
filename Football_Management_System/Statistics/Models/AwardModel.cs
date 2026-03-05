namespace Football_Management_System.Statistics.Models
{
    public class PlayerStatsEntry
    {
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public string Position { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int MinutesPlayed { get; set; }
    }
}
