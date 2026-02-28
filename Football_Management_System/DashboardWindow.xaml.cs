using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using Football_Management_System.DataAccess;

namespace Football_Management_System
{
    public partial class DashboardWindow : Window
    {
        private string currentUser;

        public DashboardWindow(string fullName)
        {
            InitializeComponent();
            currentUser = fullName;
            txtWelcome.Text = currentUser;
            txtDateTime.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy");

            string initial = !string.IsNullOrEmpty(currentUser) ? currentUser.Substring(0, 1).ToUpper() : "U";
            txtAvatar.Text = initial;
            txtHeaderAvatar.Text = initial;

            LoadOverview();
        }

        private void LoadOverview()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    int tournaments = db.Tournaments.Count();
                    int teams = db.Teams.Count();
                    int players = db.Players.Count();
                    int matches = db.Matches.Count();
                    int coaches = db.Coaches.Count();
                    int standings = db.Standings.Count();
                    int totalGoals = db.PlayerStatistics.Any()
                        ? db.PlayerStatistics.Sum(ps => (int?)(ps.Goals ?? 0)) ?? 0
                        : 0;

                    txtTournaments.Text = tournaments.ToString();
                    txtTeams.Text = teams.ToString();
                    txtPlayers.Text = players.ToString();
                    txtMatches.Text = matches.ToString();

                    txtFeedTournament.Text = tournaments + " giải đấu";
                    txtFeedCoach.Text = coaches + " huấn luyện viên";
                    txtFeedGoals.Text = totalGoals + " bàn thắng";
                    txtFeedStandings.Text = standings + " bảng xếp hạng";

                    var recentMatches = db.Matches
                        .Include(m => m.HomeTeam)
                        .Include(m => m.AwayTeam)
                        .Include(m => m.MatchResultItems)
                        .OrderByDescending(m => m.MatchDate)
                        .Take(5)
                        .ToList();
                    dgRecentMatches.ItemsSource = recentMatches;

                    var teamOverview = db.Teams.Select(t => new
                    {
                        t.TeamName,
                        t.Stadium,
                        PlayerCount = t.Players.Count(),
                        MatchCount = t.HomeMatches.Count() + t.AwayMatches.Count(),
                        Status = t.Status ?? "N/A"
                    }).OrderByDescending(t => t.PlayerCount).ToList();
                    dgTeamOverview.ItemsSource = teamOverview;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải tổng quan: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            LoadOverview();
        }

        private void btnQuanLyGiaiDau_Click(object sender, RoutedEventArgs e)
        {
            var win = new QuanLyGiaiDau();
            win.ShowDialog();
            LoadOverview();
        }

        private void btnQuanLyDoiBong_Click(object sender, RoutedEventArgs e)
        {
            var win = new QuanLyDoiBong();
            win.ShowDialog();
            LoadOverview();
        }

        private void btnQuanLyCauThu_Click(object sender, RoutedEventArgs e)
        {
            var win = new PlayerManagement();
            win.ShowDialog();
            LoadOverview();
        }

        private void btnQuanLyHLV_Click(object sender, RoutedEventArgs e)
        {
            var win = new CoachManagementWindow();
            win.ShowDialog();
            LoadOverview();
        }

        private void btnLichThiDau_Click(object sender, RoutedEventArgs e)
        {
            var win = new LichThiDau();
            win.ShowDialog();
            LoadOverview();
        }

        private void btnKetQua_Click(object sender, RoutedEventArgs e)
        {
            var win = new MatchResultWindow();
            win.ShowDialog();
            LoadOverview();
        }

        private void btnBangXepHang_Click(object sender, RoutedEventArgs e)
        {
            var win = new StandingsWindow();
            win.ShowDialog();
        }

        private void btnThongKe_Click(object sender, RoutedEventArgs e)
        {
            var win = new StatisticsWindow();
            win.ShowDialog();
        }

        private void btnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            var win = new ReportWindow();
            win.ShowDialog();
        }

        private void btnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}
