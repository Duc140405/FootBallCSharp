using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Football_Management_System
{
    public partial class StandingsWindow : Window
    {
        private string connectionString = "Server=localhost;Database=FootballManagementDB;Integrated Security=True;";
        private ObservableCollection<StandingItem> standingsList = new ObservableCollection<StandingItem>();
        private int selectedTournamentId = 1;

        public StandingsWindow()
        {
            InitializeComponent();
            LoadTournaments();
            LoadStandings();
        }

        private void LoadTournaments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetTournaments", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = cmd.ExecuteReader();
                    cboTournament.Items.Clear();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["TournamentName"].ToString(),
                            Tag = reader["TournamentID"]
                        };
                        cboTournament.Items.Add(item);
                    }

                    if (cboTournament.Items.Count > 0)
                        cboTournament.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Lỗi: {ex.Message}";
            }
        }

        private void LoadRounds()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetRoundsByTournament", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TournamentID", selectedTournamentId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    cboRound.Items.Clear();
                    cboRound.Items.Add(new ComboBoxItem { Content = "Tất cả", Tag = 0 });

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["RoundName"].ToString(),
                            Tag = reader["RoundID"]
                        };
                        cboRound.Items.Add(item);
                    }

                    if (cboRound.Items.Count > 0)
                        cboRound.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Lỗi: {ex.Message}";
            }
        }

        private void LoadStandings()
        {
            try
            {
                standingsList.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetStandings", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TournamentID", selectedTournamentId);
                    cmd.Parameters.AddWithValue("@RoundID", DBNull.Value);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        standingsList.Add(new StandingItem
                        {
                            Position = Convert.ToInt32(reader["STT"]),
                            TeamName = reader["Đội Bóng"].ToString(),
                            MatchesPlayed = Convert.ToInt32(reader["Số Trận"]),
                            Wins = Convert.ToInt32(reader["Thắng"]),
                            Draws = Convert.ToInt32(reader["Hòa"]),
                            Losses = Convert.ToInt32(reader["Thua"]),
                            GoalsFor = Convert.ToInt32(reader["BT"]),
                            GoalsAgainst = Convert.ToInt32(reader["BB"]),
                            GoalDifference = Convert.ToInt32(reader["HS"]),
                            Points = Convert.ToInt32(reader["Điểm"])
                        });
                    }
                }

                dgStandings.ItemsSource = standingsList;
                txtStatus.Text = $"Đã tải {standingsList.Count} đội bóng";
                txtUpdateTime.Text = $"Cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Lỗi: {ex.Message}";
            }
        }

        private void cboTournament_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTournament.SelectedItem is ComboBoxItem item)
            {
                selectedTournamentId = Convert.ToInt32(item.Tag);
                LoadRounds();
                LoadStandings();
            }
        }

        private void cboRound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStandings();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadStandings();
                return;
            }

            try
            {
                standingsList.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_SearchTeamInStandings", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TournamentID", selectedTournamentId);
                    cmd.Parameters.AddWithValue("@SearchText", searchText);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        standingsList.Add(new StandingItem
                        {
                            Position = Convert.ToInt32(reader["STT"]),
                            TeamName = reader["Đội Bóng"].ToString(),
                            MatchesPlayed = Convert.ToInt32(reader["Số Trận"]),
                            Wins = Convert.ToInt32(reader["Thắng"]),
                            Draws = Convert.ToInt32(reader["Hòa"]),
                            Losses = Convert.ToInt32(reader["Thua"]),
                            GoalsFor = Convert.ToInt32(reader["BT"]),
                            GoalsAgainst = Convert.ToInt32(reader["BB"]),
                            GoalDifference = Convert.ToInt32(reader["HS"]),
                            Points = Convert.ToInt32(reader["Điểm"])
                        });
                    }
                }

                dgStandings.ItemsSource = standingsList;
                txtStatus.Text = $"Tìm thấy {standingsList.Count} kết quả";
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Lỗi: {ex.Message}";
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadStandings();
            txtStatus.Text = "Đã làm mới dữ liệu";
        }

        private void btnRecalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_RecalculateStandings", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TournamentID", selectedTournamentId);
                    cmd.ExecuteNonQuery();
                }

                LoadStandings();
                txtStatus.Text = "Đã tính lại bảng xếp hạng";
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Lỗi: {ex.Message}";
            }
        }
    }

    public class StandingItem
    {
        public int Position { get; set; }
        public string TeamName { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }
    }
}
