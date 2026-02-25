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

namespace Football_Management_System
{
    /// <summary>
    /// Interaction logic for QuanLyGiaiDau.xaml
    /// </summary>
    public partial class QuanLyGiaiDau : Window
    {
        // Danh sách quan sát để tự động cập nhật UI
        public ObservableCollection<GiaiDau_Model> DanhSachGiaiDau { get; set; }

        public QuanLyGiaiDau()
        {
            InitializeComponent();
            DanhSachGiaiDau = new ObservableCollection<GiaiDau_Model>();
            dgvGiaiDau.ItemsSource = DanhSachGiaiDau;
        }

        // ĐÂY LÀ HÀM BẠN ĐANG THIẾU: Đổ dữ liệu từ bảng lên các ô nhập khi chọn dòng
        private void dgvGiaiDau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is GiaiDau_Model selected)
            {
                txtTenGiaiDau.Text = selected.TenGiai;
                txtSoVongDau.Text = selected.SoVong;

                if (DateTime.TryParse(selected.NgayBD, out DateTime db))
                    dtpNgayBatDau.SelectedDate = db;

                if (DateTime.TryParse(selected.NgayKT, out DateTime dk))
                    dtpNgayKetThuc.SelectedDate = dk;
            }
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenGiaiDau.Text)) return;

            DanhSachGiaiDau.Add(new GiaiDau_Model
            {
                TenGiai = txtTenGiaiDau.Text,
                SoVong = txtSoVongDau.Text,
                NgayBD = dtpNgayBatDau.SelectedDate?.ToString("dd/MM/yyyy") ?? "",
                NgayKT = dtpNgayKetThuc.SelectedDate?.ToString("dd/MM/yyyy") ?? ""
            });
            ClearInputs();
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is GiaiDau_Model selected)
            {
                selected.TenGiai = txtTenGiaiDau.Text;
                selected.SoVong = txtSoVongDau.Text;
                selected.NgayBD = dtpNgayBatDau.SelectedDate?.ToString("dd/MM/yyyy") ?? "";
                selected.NgayKT = dtpNgayKetThuc.SelectedDate?.ToString("dd/MM/yyyy") ?? "";

                dgvGiaiDau.Items.Refresh();
                MessageBox.Show("Cập nhật thành công!");
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgvGiaiDau.SelectedItem is GiaiDau_Model selected)
            {
                DanhSachGiaiDau.Remove(selected);
                ClearInputs();
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

    // Class dữ liệu hỗ trợ Binding
    public class GiaiDau_Model : INotifyPropertyChanged
    {
        private string _tenGiai, _soVong, _ngayBD, _ngayKT;

        public string TenGiai { get => _tenGiai; set { _tenGiai = value; OnPropertyChanged(); } }
        public string SoVong { get => _soVong; set { _soVong = value; OnPropertyChanged(); } }
        public string NgayBD { get => _ngayBD; set { _ngayBD = value; OnPropertyChanged(); } }
        public string NgayKT { get => _ngayKT; set { _ngayKT = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

