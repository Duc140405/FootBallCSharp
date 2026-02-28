using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class CoachManagementWindow : Window
    {
        private Coach selectedCoach = null;

        public CoachManagementWindow()
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
                    dgCoaches.ItemsSource = db.Coaches.ToList();
                    cboTeam.ItemsSource = db.Teams.ToList();
                }
                txtStatus.Text = "Da tai du lieu.";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Loi tai du lieu: " + ex.Message;
            }
        }

        private void dgCoaches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCoaches.SelectedItem is Coach coach)
            {
                selectedCoach = coach;
                txtCoachName.Text = coach.CoachName;
                dpBirthDate.SelectedDate = coach.BirthDate;
                txtNationality.Text = coach.Nationality;
                txtExperience.Text = coach.ExperienceYears.HasValue ? coach.ExperienceYears.ToString() : "";
                cboTeam.SelectedValue = coach.CurrentTeamID;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCoachName.Text))
            {
                txtStatus.Text = "Vui long nhap ten HLV!";
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var coach = new Coach
                    {
                        CoachName = txtCoachName.Text.Trim(),
                        BirthDate = dpBirthDate.SelectedDate,
                        Nationality = txtNationality.Text.Trim(),
                        ExperienceYears = int.TryParse(txtExperience.Text, out int exp) ? exp : (int?)null,
                        CurrentTeamID = cboTeam.SelectedValue as int?,
                        CreatedDate = DateTime.Now
                    };
                    db.Coaches.Add(coach);
                    db.SaveChanges();
                    txtStatus.Text = "Da them HLV: " + coach.CoachName;
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
            if (selectedCoach == null)
            {
                txtStatus.Text = "Vui long chon HLV can sua!";
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var coach = db.Coaches.Find(selectedCoach.CoachID);
                    if (coach != null)
                    {
                        coach.CoachName = txtCoachName.Text.Trim();
                        coach.BirthDate = dpBirthDate.SelectedDate;
                        coach.Nationality = txtNationality.Text.Trim();
                        coach.ExperienceYears = int.TryParse(txtExperience.Text, out int exp) ? exp : (int?)null;
                        coach.CurrentTeamID = cboTeam.SelectedValue as int?;
                        db.SaveChanges();
                        txtStatus.Text = "Da cap nhat HLV: " + coach.CoachName;
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
            if (selectedCoach == null)
            {
                txtStatus.Text = "Vui long chon HLV can xoa!";
                return;
            }

            var result = MessageBox.Show("Ban co chac muon xoa HLV: " + selectedCoach.CoachName + "?",
                "Xac nhan", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new FootballDbContext())
                    {
                        var coach = db.Coaches.Find(selectedCoach.CoachID);
                        if (coach != null)
                        {
                            db.Coaches.Remove(coach);
                            db.SaveChanges();
                            txtStatus.Text = "Da xoa HLV.";
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
            selectedCoach = null;
            dgCoaches.SelectedItem = null;
            txtCoachName.Text = "";
            dpBirthDate.SelectedDate = null;
            txtNationality.Text = "";
            txtExperience.Text = "";
            cboTeam.SelectedItem = null;
        }
    }
}
