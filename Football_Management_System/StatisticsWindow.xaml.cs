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
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
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
                    list.Insert(0, new Tournament { TournamentID = 0, TournamentName = "-- Tat ca --" });
                    cboTournament.ItemsSource = list;
                    cboTournament.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tai giai dau: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    int tournamentId = 0;
                    if (cboTournament.SelectedValue != null)
                        tournamentId = (int)cboTournament.SelectedValue;

                    var statsQuery = db.PlayerStatistics
                        .Include(ps => ps.Player)
                        .Include(ps => ps.Player.Team)
                        .AsQueryable();

                    if (tournamentId > 0)
                        statsQuery = statsQuery.Where(ps => ps.TournamentID == tournamentId);

                    var statsList = statsQuery.ToList();

                    int totalMatches = 0;
                    if (tournamentId > 0)
                        totalMatches = db.Matches.Count(m => m.Round != null && m.Round.TournamentID == tournamentId);
                    else
                        totalMatches = db.Matches.Count();

                    int totalGoals = statsList.Sum(ps => ps.Goals ?? 0);
                    int totalYellow = statsList.Sum(ps => ps.YellowCards ?? 0);
                    int totalRed = statsList.Sum(ps => ps.RedCards ?? 0);

                    txtTotalMatches.Text = totalMatches.ToString();
                    txtTotalGoals.Text = totalGoals.ToString();
                    txtTotalYellow.Text = totalYellow.ToString();
                    txtTotalRed.Text = totalRed.ToString();

                    var playerGroups = statsList
                        .GroupBy(ps => new { ps.PlayerID, ps.Player.PlayerName, TeamName = ps.Player.Team != null ? ps.Player.Team.TeamName : "", ps.Player.Position })
                        .Select(g => new
                        {
                            g.Key.PlayerName,
                            g.Key.TeamName,
                            g.Key.Position,
                            Goals = g.Sum(x => x.Goals ?? 0),
                            Assists = g.Sum(x => x.Assists ?? 0),
                            YellowCards = g.Sum(x => x.YellowCards ?? 0),
                            RedCards = g.Sum(x => x.RedCards ?? 0),
                            MinutesPlayed = g.Sum(x => x.MinutesPlayed ?? 0)
                        }).ToList();

                    dgTopScorers.ItemsSource = playerGroups
                        .OrderByDescending(p => p.Goals)
                        .Take(10).ToList();

                    dgTopAssists.ItemsSource = playerGroups
                        .OrderByDescending(p => p.Assists)
                        .Take(10).ToList();

                    dgTopCards.ItemsSource = playerGroups
                        .OrderByDescending(p => p.YellowCards + p.RedCards)
                        .Take(10).ToList();

                    dgBestXI.ItemsSource = playerGroups
                        .OrderByDescending(p => p.MinutesPlayed)
                        .Take(11).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tai thong ke: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cboTournament_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                LoadData();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
