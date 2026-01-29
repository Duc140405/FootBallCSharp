using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Football_Management_System
{
    public partial class MatchResultWindow : Window
    {
        private ObservableCollection<Match> danhSachTranDau = new ObservableCollection<Match>();
        private List<Match> danhSachGoc = new List<Match>();
        private Match tranDauDangChon = null;

        public MatchResultWindow()
        {
            InitializeComponent();
            LoadData();
            CapNhatThongKe();
        }

        private void LoadData()
        {
            cboMatch.ItemsSource = danhSachTranDau;
            cboMatch.DisplayMemberPath = "DisplayName";
            dgMatches.ItemsSource = danhSachTranDau;
        }

        private void CapNhatThongKe()
        {
            txtTongTran.Text = danhSachTranDau.Count.ToString();
            txtDaCoKQ.Text = danhSachTranDau.Count(m => m.HomeScore.HasValue).ToString();
        }

        private void cboMatch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMatch.SelectedItem is Match match)
            {
                HienThiThongTin(match);
            }
        }

        private void dgMatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMatches.SelectedItem is Match match)
            {
                tranDauDangChon = match;
                cboMatch.SelectedItem = match;
                HienThiThongTin(match);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(keyword))
            {
                dgMatches.ItemsSource = danhSachTranDau;
            }
            else
            {
                var ketQua = danhSachTranDau.Where(m => 
                    m.HomeTeam.ToLower().Contains(keyword) || 
                    m.AwayTeam.ToLower().Contains(keyword)).ToList();
                dgMatches.ItemsSource = ketQua;
            }
        }

        private void HienThiThongTin(Match match)
        {
            tranDauDangChon = match;
            txtHomeTeam.Text = match.HomeTeam;
            txtAwayTeam.Text = match.AwayTeam;
            dpMatchDate.SelectedDate = match.MatchDate;
            txtHomeScore.Text = match.HomeScore?.ToString() ?? "";
            txtAwayScore.Text = match.AwayScore?.ToString() ?? "";
            txtHomeYellow.Text = match.HomeYellowCards?.ToString() ?? "";
            txtAwayYellow.Text = match.AwayYellowCards?.ToString() ?? "";
            txtHomeRed.Text = match.HomeRedCards?.ToString() ?? "";
            txtAwayRed.Text = match.AwayRedCards?.ToString() ?? "";
            txtNote.Text = match.Note ?? "";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHomeTeam.Text))
            {
                txtStatus.Text = "⚠️ Vui lòng nhập tên đội nhà!";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAwayTeam.Text))
            {
                txtStatus.Text = "⚠️ Vui lòng nhập tên đội khách!";
                return;
            }
            if (dpMatchDate.SelectedDate == null)
            {
                txtStatus.Text = "⚠️ Vui lòng chọn ngày thi đấu!";
                return;
            }

            Match tranMoi = new Match
            {
                MatchId = danhSachTranDau.Count + 1,
                HomeTeam = txtHomeTeam.Text.Trim(),
                AwayTeam = txtAwayTeam.Text.Trim(),
                MatchDate = dpMatchDate.SelectedDate.Value
            };

            if (int.TryParse(txtHomeScore.Text, out int homeScore))
                tranMoi.HomeScore = homeScore;
            if (int.TryParse(txtAwayScore.Text, out int awayScore))
                tranMoi.AwayScore = awayScore;

            if (int.TryParse(txtHomeYellow.Text, out int hy))
                tranMoi.HomeYellowCards = hy;
            if (int.TryParse(txtAwayYellow.Text, out int ay))
                tranMoi.AwayYellowCards = ay;
            if (int.TryParse(txtHomeRed.Text, out int hr))
                tranMoi.HomeRedCards = hr;
            if (int.TryParse(txtAwayRed.Text, out int ar))
                tranMoi.AwayRedCards = ar;

            tranMoi.Note = txtNote.Text.Trim();

            danhSachTranDau.Add(tranMoi);
            danhSachGoc.Add(tranMoi);
            
            txtStatus.Text = $"✅ Đã thêm trận: {tranMoi.HomeTeam} vs {tranMoi.AwayTeam}";
            CapNhatThongKe();
            XoaForm();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (tranDauDangChon == null)
            {
                txtStatus.Text = "⚠️ Vui lòng chọn trận đấu cần sửa!";
                return;
            }

            tranDauDangChon.HomeTeam = txtHomeTeam.Text.Trim();
            tranDauDangChon.AwayTeam = txtAwayTeam.Text.Trim();
            
            if (dpMatchDate.SelectedDate != null)
                tranDauDangChon.MatchDate = dpMatchDate.SelectedDate.Value;

            if (int.TryParse(txtHomeScore.Text, out int homeScore))
                tranDauDangChon.HomeScore = homeScore;
            else
                tranDauDangChon.HomeScore = null;

            if (int.TryParse(txtAwayScore.Text, out int awayScore))
                tranDauDangChon.AwayScore = awayScore;
            else
                tranDauDangChon.AwayScore = null;

            if (int.TryParse(txtHomeYellow.Text, out int hy))
                tranDauDangChon.HomeYellowCards = hy;
            else
                tranDauDangChon.HomeYellowCards = null;

            if (int.TryParse(txtAwayYellow.Text, out int ay))
                tranDauDangChon.AwayYellowCards = ay;
            else
                tranDauDangChon.AwayYellowCards = null;

            if (int.TryParse(txtHomeRed.Text, out int hr))
                tranDauDangChon.HomeRedCards = hr;
            else
                tranDauDangChon.HomeRedCards = null;

            if (int.TryParse(txtAwayRed.Text, out int ar))
                tranDauDangChon.AwayRedCards = ar;
            else
                tranDauDangChon.AwayRedCards = null;

            tranDauDangChon.Note = txtNote.Text.Trim();

            dgMatches.Items.Refresh();
            CapNhatThongKe();
            
            txtStatus.Text = $"✅ Đã sửa trận: {tranDauDangChon.HomeTeam} vs {tranDauDangChon.AwayTeam}";
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tranDauDangChon == null)
            {
                txtStatus.Text = "⚠️ Vui lòng chọn trận đấu cần xóa!";
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa trận:\n{tranDauDangChon.HomeTeam} vs {tranDauDangChon.AwayTeam}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string tenTran = $"{tranDauDangChon.HomeTeam} vs {tranDauDangChon.AwayTeam}";
                danhSachTranDau.Remove(tranDauDangChon);
                danhSachGoc.Remove(tranDauDangChon);
                txtStatus.Text = $"🗑️ Đã xóa trận: {tenTran}";
                CapNhatThongKe();
                XoaForm();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            XoaForm();
            txtStatus.Text = "🔄 Đã làm mới form!";
        }

        private void XoaForm()
        {
            tranDauDangChon = null;
            cboMatch.SelectedItem = null;
            dgMatches.SelectedItem = null;
            txtHomeTeam.Text = "";
            txtAwayTeam.Text = "";
            dpMatchDate.SelectedDate = null;
            txtHomeScore.Text = "";
            txtAwayScore.Text = "";
            txtHomeYellow.Text = "";
            txtAwayYellow.Text = "";
            txtHomeRed.Text = "";
            txtAwayRed.Text = "";
            txtNote.Text = "";
        }
    }

    public class Match
    {
        public int MatchId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime MatchDate { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public int? HomeYellowCards { get; set; }
        public int? AwayYellowCards { get; set; }
        public int? HomeRedCards { get; set; }
        public int? AwayRedCards { get; set; }
        public string Note { get; set; }

        public string DisplayName => $"{HomeTeam} vs {AwayTeam} ({MatchDate:dd/MM/yyyy})";
        public string Result => HomeScore.HasValue ? $"{HomeScore} - {AwayScore}" : "Chưa có";
        public string TotalYellow => (HomeYellowCards ?? 0) + (AwayYellowCards ?? 0) > 0 
            ? ((HomeYellowCards ?? 0) + (AwayYellowCards ?? 0)).ToString() : "-";
        public string TotalRed => (HomeRedCards ?? 0) + (AwayRedCards ?? 0) > 0 
            ? ((HomeRedCards ?? 0) + (AwayRedCards ?? 0)).ToString() : "-";
        public string Status => HomeScore.HasValue ? "✅ Hoàn thành" : "⏳ Chờ KQ";
    }
}
