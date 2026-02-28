using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class PlayerManagement : Window
    {
        private Player selectedPlayer = null;

        public PlayerManagement()
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
                    dgPlayers.ItemsSource = db.Players.Include(p => p.Team).ToList();
                    cboTeam.ItemsSource = db.Teams.ToList();
                }
                txtStatus.Text = "Da tai du lieu.";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Loi tai du lieu: " + ex.Message;
            }
        }

        private void dgPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPlayers.SelectedItem is Player player)
            {
                selectedPlayer = player;
                txtPlayerName.Text = player.PlayerName;
                cboTeam.SelectedValue = player.TeamID;
                dpDateOfBirth.SelectedDate = player.DateOfBirth;
                txtNationality.Text = player.Nationality;
                txtJerseyNumber.Text = player.JerseyNumber.HasValue ? player.JerseyNumber.ToString() : "";

                foreach (ComboBoxItem item in cboPosition.Items)
                {
                    if (item.Content.ToString() == player.Position)
                    {
                        cboPosition.SelectedItem = item;
                        break;
                    }
                }

                foreach (ComboBoxItem item in cboStatus.Items)
                {
                    if (item.Content.ToString() == player.Status)
                    {
                        cboStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                txtStatus.Text = "Vui long nhap ten cau thu!";
                return;
            }
            if (cboTeam.SelectedValue == null)
            {
                txtStatus.Text = "Vui long chon doi bong!";
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var player = new Player
                    {
                        PlayerName = txtPlayerName.Text.Trim(),
                        TeamID = (int)cboTeam.SelectedValue,
                        DateOfBirth = dpDateOfBirth.SelectedDate,
                        Nationality = txtNationality.Text.Trim(),
                        JerseyNumber = int.TryParse(txtJerseyNumber.Text, out int jn) ? jn : (int?)null,
                        Position = (cboPosition.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        CreatedDate = DateTime.Now
                    };
                    db.Players.Add(player);
                    db.SaveChanges();
                    txtStatus.Text = "Da them cau thu: " + player.PlayerName;
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
            if (selectedPlayer == null)
            {
                txtStatus.Text = "Vui long chon cau thu can sua!";
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var player = db.Players.Find(selectedPlayer.PlayerID);
                    if (player != null)
                    {
                        player.PlayerName = txtPlayerName.Text.Trim();
                        player.TeamID = (int)cboTeam.SelectedValue;
                        player.DateOfBirth = dpDateOfBirth.SelectedDate;
                        player.Nationality = txtNationality.Text.Trim();
                        player.JerseyNumber = int.TryParse(txtJerseyNumber.Text, out int jn) ? jn : (int?)null;
                        player.Position = (cboPosition.SelectedItem as ComboBoxItem)?.Content.ToString();
                        player.Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
                        db.SaveChanges();
                        txtStatus.Text = "Da cap nhat cau thu: " + player.PlayerName;
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
            if (selectedPlayer == null)
            {
                txtStatus.Text = "Vui long chon cau thu can xoa!";
                return;
            }

            var result = MessageBox.Show("Ban co chac muon xoa cau thu: " + selectedPlayer.PlayerName + "?",
                "Xac nhan", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new FootballDbContext())
                    {
                        var player = db.Players.Find(selectedPlayer.PlayerID);
                        if (player != null)
                        {
                            db.Players.Remove(player);
                            db.SaveChanges();
                            txtStatus.Text = "Da xoa cau thu.";
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
            selectedPlayer = null;
            dgPlayers.SelectedItem = null;
            txtPlayerName.Text = "";
            cboTeam.SelectedItem = null;
            dpDateOfBirth.SelectedDate = null;
            txtNationality.Text = "";
            txtJerseyNumber.Text = "";
            cboPosition.SelectedItem = null;
            cboStatus.SelectedItem = null;
        }
    }
}
