using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class StandingsWindow : Window
    {
        private List<Standing> allStandings = new List<Standing>();

        public StandingsWindow()
        {
            InitializeComponent();
            LoadTournaments();
            LoadData();
        }

        private void LoadTournaments()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    var list = db.Tournaments.OrderBy(t => t.TournamentName).ToList();
                    list.Insert(0, new Tournament { TournamentID = 0, TournamentName = "-- Tất cả --" });
                    cboTournament.ItemsSource = list;
                    cboTournament.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải giải đấu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    var query = db.Standings.Include(s => s.Team).Include(s => s.Tournament).AsQueryable();

                    if (cboTournament.SelectedValue != null)
                    {
                        int tournamentId = (int)cboTournament.SelectedValue;
                        if (tournamentId > 0)
                        {
                            query = query.Where(s => s.TournamentID == tournamentId);
                        }
                    }

                    allStandings = query.OrderByDescending(s => s.Points)
                                       .ThenByDescending(s => s.GoalDifference)
                                       .ThenByDescending(s => s.GoalsFor)
                                       .ToList();

                    ApplyFilter();
                    txtStatus.Text = "Đã tải " + allStandings.Count + " bản ghi.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng xếp hạng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            string keyword = txtSearch.Text?.Trim().ToLower() ?? "";
            var filtered = allStandings.AsEnumerable();

            if (!string.IsNullOrEmpty(keyword))
            {
                filtered = filtered.Where(s => s.Team != null && s.Team.TeamName != null && s.Team.TeamName.ToLower().Contains(keyword));
            }

            dgStandings.ItemsSource = filtered.ToList();
        }

        private void cboTournament_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                LoadData();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadData();
        }
    }
}
