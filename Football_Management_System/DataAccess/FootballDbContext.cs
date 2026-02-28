using System.Data.Entity;
using Football_Management_System.Models;

namespace Football_Management_System.DataAccess
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext() : base("name=FootballDB")
        {
            Database.SetInitializer<FootballDbContext>(null);
        }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<CoachHistory> CoachHistories { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<PlayerGeneralStatistic> PlayerGeneralStatistics { get; set; }
        public DbSet<PlayerNote> PlayerNotes { get; set; }
        public DbSet<PlayerAttachment> PlayerAttachments { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }
        public DbSet<Standing> Standings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>()
                .HasRequired(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Match>()
                .HasRequired(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MatchResult>()
                .HasRequired(mr => mr.Match)
                .WithMany(m => m.MatchResultItems)
                .HasForeignKey(mr => mr.MatchID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
