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
    public partial class LichThiDau : Window
    {
        private List<Match> allMatches = new List<Match>();

        public LichThiDau()
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
                    var list = db.Tournaments.ToList();
                    list.Insert(0, new Tournament { TournamentID = 0, TournamentName = "Tat ca" });
                    cboTournament.ItemsSource = list;
                    cboTournament.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Loi tai giai dau: " + ex.Message;
            }
        }

        private void LoadData()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    allMatches = db.Matches
                        .Include(m => m.HomeTeam)
                        .Include(m => m.AwayTeam)
                        .Include(m => m.MatchResultItems)
                        .OrderByDescending(m => m.MatchDate)
                        .ToList();
                }
                dgSchedule.ItemsSource = allMatches;
                txtStatus.Text = "Tong so tran: " + allMatches.Count;
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Loi tai du lieu: " + ex.Message;
            }
        }

        private void ApplyFilter()
        {
            var filtered = allMatches.AsEnumerable();

            if (cboTournament.SelectedValue is int tournamentId && tournamentId > 0)
            {
                filtered = filtered.Where(m => m.TournamentID == tournamentId);
            }

            string keyword = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(keyword))
            {
                filtered = filtered.Where(m =>
                    m.HomeTeamName.ToLower().Contains(keyword) ||
                    m.AwayTeamName.ToLower().Contains(keyword));
            }

            dgSchedule.ItemsSource = filtered.ToList();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void cboTournament_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            txtSearch.Text = "";
            cboTournament.SelectedIndex = 0;
            txtStatus.Text = "Da lam moi du lieu!";
        }
    }
}
