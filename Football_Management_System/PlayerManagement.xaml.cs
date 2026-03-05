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
                    // Lấy danh sách cầu thủ và đội bóng
                    dgPlayers.ItemsSource = db.Players.Include(p => p.Team).ToList();
                    cboTeam.ItemsSource = db.Teams.ToList();
                }
                ShowStatus("Đã tải dữ liệu thành công.", false);
            }
            catch (Exception ex)
            {
                ShowStatus("Lỗi tải dữ liệu: " + ex.Message, true);
                MessageBox.Show("Không thể kết nối CSDL:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

                // Tối ưu: Tìm và chọn item trong ComboBox không cần vòng lặp foreach
                cboPosition.SelectedItem = cboPosition.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString().StartsWith(player.Position ?? ""));

                cboStatus.SelectedItem = cboStatus.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == player.Status);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

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
                        // Cắt chuỗi để chỉ lấy "GK", "DF" (nếu bạn dùng giao diện XAML mới có chứa tiếng Việt)
                        Position = GetSelectedComboBoxString(cboPosition)?.Split(' ')[0],
                        Status = GetSelectedComboBoxString(cboStatus),
                        CreatedDate = DateTime.Now
                    };

                    db.Players.Add(player);
                    db.SaveChanges();
                    ShowStatus($"Đã thêm cầu thủ: {player.PlayerName}", false);
                }
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowStatus("Lỗi thêm cầu thủ.", true);
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPlayer == null)
            {
                MessageBox.Show("Vui lòng chọn cầu thủ cần sửa từ danh sách!", "Chú ý", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput()) return;

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
                        player.Position = GetSelectedComboBoxString(cboPosition)?.Split(' ')[0];
                        player.Status = GetSelectedComboBoxString(cboStatus);

                        db.SaveChanges();
                        ShowStatus($"Đã cập nhật cầu thủ: {player.PlayerName}", false);
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                ShowStatus("Lỗi cập nhật dữ liệu.", true);
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPlayer == null)
            {
                MessageBox.Show("Vui lòng chọn cầu thủ cần xóa!", "Chú ý", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa cầu thủ: {selectedPlayer.PlayerName}?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                            ShowStatus("Đã xóa cầu thủ thành công.", false);
                        }
                    }
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    ShowStatus("Lỗi xóa cầu thủ.", true);
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            ShowStatus("Đã làm mới Form.", false);
        }

        // --- CÁC HÀM HỖ TRỢ (HELPER METHODS) ---

        private void ClearForm()
        {
            selectedPlayer = null;
            dgPlayers.SelectedItem = null;
            txtPlayerName.Text = string.Empty;
            cboTeam.SelectedItem = null;
            dpDateOfBirth.SelectedDate = null;
            txtNationality.Text = string.Empty;
            txtJerseyNumber.Text = string.Empty;
            cboPosition.SelectedItem = null;
            cboStatus.SelectedItem = null;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên cầu thủ!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPlayerName.Focus();
                return false;
            }
            if (cboTeam.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đội bóng!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                cboTeam.Focus();
                return false;
            }
            return true;
        }

        private string GetSelectedComboBoxString(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
        }

        private void ShowStatus(string message, bool isError)
        {
            if (txtStatus != null)
            {
                txtStatus.Text = message;
                txtStatus.Foreground = isError ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.LightGreen;
            }
        }
    }
}