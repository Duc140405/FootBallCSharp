using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class QuanLyGiaiDau : Window
    {
        public QuanLyGiaiDau()
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
                    dgvGiaiDau.ItemsSource = db.Tournaments.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tai du lieu: " + ex.Message);
            }
        }

        private void dgvGiaiDau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is Tournament selected)
            {
                txtTenGiaiDau.Text = selected.TournamentName;
                txtSoVongDau.Text = selected.TotalRounds.ToString();
                dtpNgayBatDau.SelectedDate = selected.StartDate;
                dtpNgayKetThuc.SelectedDate = selected.EndDate;
            }
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenGiaiDau.Text)) return;

            try
            {
                using (var db = new FootballDbContext())
                {
                    var moi = new Tournament
                    {
                        TournamentName = txtTenGiaiDau.Text,
                        TotalRounds = int.TryParse(txtSoVongDau.Text, out int sv) ? sv : 0,
                        StartDate = dtpNgayBatDau.SelectedDate,
                        EndDate = dtpNgayKetThuc.SelectedDate,
                        Status = "Dang dien ra"
                    };

                    db.Tournaments.Add(moi);
                    db.SaveChanges();
                }

                LoadData();
                ClearInputs();
                MessageBox.Show("Them giai dau thanh cong!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi them giai dau: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is Tournament selected)
            {
                try
                {
                    using (var db = new FootballDbContext())
                    {
                        var editItem = db.Tournaments.Find(selected.TournamentID);
                        if (editItem != null)
                        {
                            editItem.TournamentName = txtTenGiaiDau.Text;
                            editItem.TotalRounds = int.TryParse(txtSoVongDau.Text, out int sv) ? sv : 0;
                            editItem.StartDate = dtpNgayBatDau.SelectedDate;
                            editItem.EndDate = dtpNgayKetThuc.SelectedDate;

                            db.SaveChanges();
                        }
                    }

                    LoadData();
                    MessageBox.Show("Cap nhat thanh cong!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi cap nhat: " + ex.Message);
                }
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is Tournament selected)
            {
                var res = MessageBox.Show("Ban co chac muon xoa giai dau nay?", "Xac nhan", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new FootballDbContext())
                        {
                            var deleteItem = db.Tournaments.Find(selected.TournamentID);
                            if (deleteItem != null)
                            {
                                db.Tournaments.Remove(deleteItem);
                                db.SaveChanges();
                            }
                        }

                        LoadData();
                        ClearInputs();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Loi xoa: " + ex.Message);
                    }
                }
            }
        }

        private void ClearInputs()
        {
            txtTenGiaiDau.Clear();
            txtSoVongDau.Clear();
            dtpNgayBatDau.SelectedDate = null;
            dtpNgayKetThuc.SelectedDate = null;
        }
    }
}

