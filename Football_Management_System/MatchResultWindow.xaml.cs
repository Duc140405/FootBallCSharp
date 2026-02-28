using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class MatchResultWindow : Window
    {
        private ObservableCollection<Match> danhSachTranDau = new ObservableCollection<Match>();
        private Match tranDauDangChon = null;

        public MatchResultWindow()
        {
            InitializeComponent();
            LoadData();
            CapNhatThongKe();
        }

        private void LoadData()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    var matches = db.Matches
                        .Include(m => m.HomeTeam)
                        .Include(m => m.AwayTeam)
                        .Include(m => m.MatchResult)
                        .OrderByDescending(m => m.MatchDate)
                        .ToList();

                    danhSachTranDau.Clear();
                    foreach (var match in matches)
                    {
                        danhSachTranDau.Add(match);
                    }
                }

                txtStatus.Text = "✅ Đã kết nối database thành công!";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "⚠️ Lỗi tải dữ liệu: " + ex.Message;
            }

            cboMatch.ItemsSource = danhSachTranDau;
            cboMatch.DisplayMemberPath = "DisplayName";
            dgMatches.ItemsSource = danhSachTranDau;
        }

        private void CapNhatThongKe()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    int tongTran = db.Matches.Count();
                    int daCoKQ = db.MatchResults.Count();

                    txtTongTran.Text = tongTran.ToString();
                    txtDaCoKQ.Text = daCoKQ.ToString();
                }
            }
            catch
            {
                txtTongTran.Text = danhSachTranDau.Count.ToString();
                txtDaCoKQ.Text = danhSachTranDau.Count(m => m.MatchResult != null).ToString();
            }
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
                    m.HomeTeamName.ToLower().Contains(keyword) ||
                    m.AwayTeamName.ToLower().Contains(keyword)).ToList();
                dgMatches.ItemsSource = ketQua;
            }
        }

        private void HienThiThongTin(Match match)
        {
            tranDauDangChon = match;
            txtHomeTeam.Text = match.HomeTeamName;
            txtAwayTeam.Text = match.AwayTeamName;
            dpMatchDate.SelectedDate = match.MatchDate;

            if (match.MatchResult != null)
            {
                txtHomeScore.Text = match.MatchResult.HomeScore.HasValue ? match.MatchResult.HomeScore.ToString() : "";
                txtAwayScore.Text = match.MatchResult.AwayScore.HasValue ? match.MatchResult.AwayScore.ToString() : "";
                txtHomeYellow.Text = match.MatchResult.HomeYellowCards.HasValue ? match.MatchResult.HomeYellowCards.ToString() : "";
                txtAwayYellow.Text = match.MatchResult.AwayYellowCards.HasValue ? match.MatchResult.AwayYellowCards.ToString() : "";
                txtHomeRed.Text = match.MatchResult.HomeRedCards.HasValue ? match.MatchResult.HomeRedCards.ToString() : "";
                txtAwayRed.Text = match.MatchResult.AwayRedCards.HasValue ? match.MatchResult.AwayRedCards.ToString() : "";
                txtNote.Text = match.MatchResult.Note ?? "";
            }
            else
            {
                txtHomeScore.Text = "";
                txtAwayScore.Text = "";
                txtHomeYellow.Text = "";
                txtAwayYellow.Text = "";
                txtHomeRed.Text = "";
                txtAwayRed.Text = "";
                txtNote.Text = "";
            }
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

            try
            {
                using (var db = new FootballDbContext())
                {
                    string homeTeamName = txtHomeTeam.Text.Trim();
                    var homeTeam = db.Teams.FirstOrDefault(t => t.TeamName == homeTeamName);
                    if (homeTeam == null)
                    {
                        homeTeam = new Team { TeamName = homeTeamName };
                        db.Teams.Add(homeTeam);
                        db.SaveChanges();
                    }

                    string awayTeamName = txtAwayTeam.Text.Trim();
                    var awayTeam = db.Teams.FirstOrDefault(t => t.TeamName == awayTeamName);
                    if (awayTeam == null)
                    {
                        awayTeam = new Team { TeamName = awayTeamName };
                        db.Teams.Add(awayTeam);
                        db.SaveChanges();
                    }

                    var newMatch = new Match
                    {
                        HomeTeamID = homeTeam.TeamID,
                        AwayTeamID = awayTeam.TeamID,
                        MatchDate = dpMatchDate.SelectedDate.Value
                    };

                    db.Matches.Add(newMatch);
                    db.SaveChanges();

                    if (int.TryParse(txtHomeScore.Text, out int hs))
                    {
                        int.TryParse(txtAwayScore.Text, out int aws);
                        int.TryParse(txtHomeYellow.Text, out int hy);
                        int.TryParse(txtAwayYellow.Text, out int ay);
                        int.TryParse(txtHomeRed.Text, out int hr);
                        int.TryParse(txtAwayRed.Text, out int ar);

                        var result = new MatchResult
                        {
                            MatchID = newMatch.MatchID,
                            HomeScore = hs,
                            AwayScore = aws,
                            HomeYellowCards = hy,
                            AwayYellowCards = ay,
                            HomeRedCards = hr,
                            AwayRedCards = ar,
                            Note = txtNote.Text.Trim()
                        };

                        db.MatchResults.Add(result);
                        db.SaveChanges();
                    }

                    txtStatus.Text = "✅ Đã thêm trận: " + homeTeamName + " vs " + awayTeamName;
                }

                LoadData();
                CapNhatThongKe();
                XoaForm();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "⚠️ Lỗi thêm trận: " + ex.Message;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (tranDauDangChon == null)
            {
                txtStatus.Text = "⚠️ Vui lòng chọn trận đấu cần sửa!";
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var match = db.Matches
                        .Include(m => m.MatchResult)
                        .FirstOrDefault(m => m.MatchID == tranDauDangChon.MatchID);

                    if (match == null)
                    {
                        txtStatus.Text = "⚠️ Không tìm thấy trận đấu!";
                        return;
                    }

                    if (dpMatchDate.SelectedDate != null)
                        match.MatchDate = dpMatchDate.SelectedDate.Value;

                    int? homeScore = null, awayScore = null;
                    int? hy = null, ay = null, hr = null, ar = null;

                    if (int.TryParse(txtHomeScore.Text, out int hsVal)) homeScore = hsVal;
                    if (int.TryParse(txtAwayScore.Text, out int asVal)) awayScore = asVal;
                    if (int.TryParse(txtHomeYellow.Text, out int hyVal)) hy = hyVal;
                    if (int.TryParse(txtAwayYellow.Text, out int ayVal)) ay = ayVal;
                    if (int.TryParse(txtHomeRed.Text, out int hrVal)) hr = hrVal;
                    if (int.TryParse(txtAwayRed.Text, out int arVal)) ar = arVal;

                    if (match.MatchResult != null)
                    {
                        match.MatchResult.HomeScore = homeScore;
                        match.MatchResult.AwayScore = awayScore;
                        match.MatchResult.HomeYellowCards = hy;
                        match.MatchResult.AwayYellowCards = ay;
                        match.MatchResult.HomeRedCards = hr;
                        match.MatchResult.AwayRedCards = ar;
                        match.MatchResult.Note = txtNote.Text.Trim();
                    }
                    else if (homeScore.HasValue)
                    {
                        var newResult = new MatchResult
                        {
                            MatchID = match.MatchID,
                            HomeScore = homeScore,
                            AwayScore = awayScore,
                            HomeYellowCards = hy,
                            AwayYellowCards = ay,
                            HomeRedCards = hr,
                            AwayRedCards = ar,
                            Note = txtNote.Text.Trim()
                        };
                        db.MatchResults.Add(newResult);
                    }

                    db.SaveChanges();

                    txtStatus.Text = "Đã cập nhật trận đấu!";
                }

                LoadData();
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "⚠️ Lỗi cập nhật: " + ex.Message;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tranDauDangChon == null)
            {
                txtStatus.Text = "⚠️ Vui lòng chọn trận đấu cần xóa!";
                return;
            }

            string tenTran = tranDauDangChon.HomeTeamName + " vs " + tranDauDangChon.AwayTeamName;

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa trận:\n" + tenTran + "?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new FootballDbContext())
                    {
                        var match = db.Matches
                            .Include(m => m.MatchResult)
                            .FirstOrDefault(m => m.MatchID == tranDauDangChon.MatchID);

                        if (match != null)
                        {
                            if (match.MatchResult != null)
                            {
                                db.MatchResults.Remove(match.MatchResult);
                            }

                            db.Matches.Remove(match);
                            db.SaveChanges();
                        }

                        txtStatus.Text = "🗑️ Đã xóa: " + tenTran;
                    }

                    LoadData();
                    CapNhatThongKe();
                    XoaForm();
                }
                catch (Exception ex)
                {
                    txtStatus.Text = "⚠️ Lỗi xóa: " + ex.Message;
                }
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
}
