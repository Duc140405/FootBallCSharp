using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Football_Management_System.DataAccess;
using Football_Management_System.Database;
using System.Data.Entity;

namespace Football_Management_System
{
    /// <summary>
    /// Interaction logic for QuanLyGiaiDau.xaml
    /// </summary>
    public partial class QuanLyGiaiDau : Window
    {
        // Khai báo Context và Helper theo quy định của nhóm
        private FootballManagementDBEntities db;
        private DatabaseHelper helper = new DatabaseHelper();

        public QuanLyGiaiDau()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
        }

        // Khởi tạo kết nối thông qua DatabaseHelper để không "hardcode" connection string
        private void InitializeDatabase()
        {
            try
            {
                var connection = helper.GetConnection();
                // Sử dụng constructor partial vừa tạo ở bước 1
                db = new FootballManagementDBEntities(connection, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Database: " + ex.Message);
            }
        }

        private void LoadData()
        {
            if (db == null) return;
            // Đổ dữ liệu thật từ bảng Tournaments của SQL Master lên bảng
            dgvGiaiDau.ItemsSource = db.Tournaments.ToList();
        }

        private void dgvGiaiDau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is Tournaments selected)
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

            var moi = new Tournaments
            {
                TournamentName = txtTenGiaiDau.Text,
                TotalRounds = int.TryParse(txtSoVongDau.Text, out int sv) ? sv : 0,
                StartDate = dtpNgayBatDau.SelectedDate,
                EndDate = dtpNgayKetThuc.SelectedDate,
                Status = "Đang diễn ra" 
            };

            db.Tournaments.Add(moi);
            db.SaveChanges(); // Lưu trực tiếp xuống SQL Server Express

            LoadData();
            ClearInputs();
            MessageBox.Show("Thêm giải đấu thành công!");
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is Tournaments selected)
            {
                var editItem = db.Tournaments.Find(selected.TournamentID);
                if (editItem != null)
                {
                    editItem.TournamentName = txtTenGiaiDau.Text;
                    editItem.TotalRounds = int.TryParse(txtSoVongDau.Text, out int sv) ? sv : 0;
                    editItem.StartDate = dtpNgayBatDau.SelectedDate;
                    editItem.EndDate = dtpNgayKetThuc.SelectedDate;

                    db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Cập nhật thành công!");
                }
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is Tournaments selected)
            {
                var res = MessageBox.Show("Bạn có chắc muốn xóa giải đấu này?", "Xác nhận", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    var deleteItem = db.Tournaments.Find(selected.TournamentID);
                    if (deleteItem != null)
                    {
                        db.Tournaments.Remove(deleteItem);
                        db.SaveChanges();
                        LoadData();
                        ClearInputs();
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

