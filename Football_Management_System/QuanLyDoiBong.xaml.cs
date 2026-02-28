using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class QuanLyDoiBong : Window
    {
        private Team selectedTeam = null;

        public QuanLyDoiBong()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    dgTeams.ItemsSource = db.Teams.ToList();
                    cboTournament.ItemsSource = db.Tournaments.ToList();
                }
                txtStatus.Text = "Da tai du lieu.";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Loi tai du lieu: " + ex.Message;
            }
        }

        private void dgTeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTeams.SelectedItem is Team team)
            {
                selectedTeam = team;
                txtTeamName.Text = team.TeamName;
                txtShortName.Text = team.ShortName;
                txtStadium.Text = team.Stadium;
                cboTournament.SelectedValue = team.TournamentID;

                foreach (ComboBoxItem item in cboStatus.Items)
                {
                    if (item.Content.ToString() == team.Status)
                    {
                        cboStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                txtStatus.Text = "Vui long nhap ten doi!";
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var team = new Team
                    {
                        TeamName = txtTeamName.Text.Trim(),
                        ShortName = txtShortName.Text.Trim(),
                        Stadium = txtStadium.Text.Trim(),
                        TournamentID = cboTournament.SelectedValue as int?,
                        Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        CreatedDate = DateTime.Now
                    };
                    db.Teams.Add(team);
                    db.SaveChanges();
                    txtStatus.Text = "Da them doi: " + team.TeamName;
                }
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Loi them: " + ex.Message;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTeam == null)
            {
                txtStatus.Text = "Vui long chon doi can sua!";
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var team = db.Teams.Find(selectedTeam.TeamID);
                    if (team != null)
                    {
                        team.TeamName = txtTeamName.Text.Trim();
                        team.ShortName = txtShortName.Text.Trim();
                        team.Stadium = txtStadium.Text.Trim();
                        team.TournamentID = cboTournament.SelectedValue as int?;
                        team.Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
                        db.SaveChanges();
                        txtStatus.Text = "Da cap nhat doi: " + team.TeamName;
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Loi cap nhat: " + ex.Message;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTeam == null)
            {
                txtStatus.Text = "Vui long chon doi can xoa!";
                return;
            }

            var result = MessageBox.Show("Ban co chac muon xoa doi: " + selectedTeam.TeamName + "?",
                "Xac nhan", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new FootballDbContext())
                    {
                        var team = db.Teams.Find(selectedTeam.TeamID);
                        if (team != null)
                        {
                            db.Teams.Remove(team);
                            db.SaveChanges();
                            txtStatus.Text = "Da xoa doi.";
                        }
                    }
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    txtStatus.Text = "Loi xoa: " + ex.Message;
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            txtStatus.Text = "Da lam moi form!";
        }

        private void ClearForm()
        {
            selectedTeam = null;
            dgTeams.SelectedItem = null;
            txtTeamName.Text = "";
            txtShortName.Text = "";
            txtStadium.Text = "";
            cboTournament.SelectedItem = null;
            cboStatus.SelectedItem = null;
        }
    }
}
