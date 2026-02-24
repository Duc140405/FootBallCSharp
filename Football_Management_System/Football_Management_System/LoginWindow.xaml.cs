using System.Collections.Generic;
using System.Windows;

namespace Football_Management_System
{
    public partial class LoginWindow : Window
    {
        private readonly Dictionary<string, (string Password, string Role)> _users = 
            new Dictionary<string, (string, string)>
        {
            { "admin", ("admin", "admin") },
            { "manager", ("minhquan ", "123") }
        };

        public static string CurrentUser { get; private set; }
        public static string CurrentRole { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (_users.TryGetValue(username.ToLower(), out var userInfo) && userInfo.Password == password)
            {
                CurrentUser = username;
                CurrentRole = userInfo.Role;

                QuanLyGiaiDau mainWindow = new QuanLyGiaiDau();
                mainWindow.Show();
                Close();
            }
            else
            {
                ShowError("Tài khoản hoặc mật khẩu không đúng!");
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}