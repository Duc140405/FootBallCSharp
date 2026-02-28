using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Hash password SHA256 (khớp với SQL Server)
        /// </summary>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Vui lòng nhập tên đăng nhập và mật khẩu!";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (var context = new FootballDbContext())
                {
                    // Hash password để so sánh
                    string hashedPassword = HashPassword(password);

                    // Query user từ database
                    var user = context.Users.FirstOrDefault(u => 
                        u.Username == username && 
                        u.IsActive == true);

                    if (user == null)
                    {
                        txtError.Text = "Tên đăng nhập không tồn tại!";
                        txtError.Visibility = Visibility.Visible;
                        return;
                    }

                    // Kiểm tra password
                    if (user.PasswordHash != hashedPassword)
                    {
                        txtError.Text = "Mật khẩu không chính xác!";
                        txtError.Visibility = Visibility.Visible;
                        return;
                    }

                    // Lưu thông tin user vào App global
                    App.CurrentUser = user;
                    App.CurrentUserRole = user.RoleID;

                    // Mở DashboardWindow
                    var dashboard = new DashboardWindow(user.FullName);
                    dashboard.Show();

                    // Đóng login window
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                txtError.Text = $"Lỗi kết nối database: {ex.Message}";
                txtError.Visibility = Visibility.Visible;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtRegUsername.Text.Trim();
            string password = txtRegPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string email = txtRegEmail.Text.Trim();
            string fullName = txtRegFullName.Text.Trim();

            // Kiểm tra dữ liệu
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu không khớp!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new FootballDbContext())
                {
                    // Check connection
                    if (!context.Database.Exists())
                    {
                        MessageBox.Show("Database không tồn tại!", "Lỗi", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Check roles
                    int roleCount = context.Roles.Count();
                    if (roleCount == 0)
                    {
                        MessageBox.Show("Roles không được khởi tạo. Chạy script FootballDB_Master.sql!", "Lỗi", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra username đã tồn tại
                    if (context.Users.Any(u => u.Username == username))
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Kiểm tra email đã tồn tại
                    if (context.Users.Any(u => u.Email == email))
                    {
                        MessageBox.Show("Email đã được đăng ký!", "Lỗi", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Tạo user mới
                    string hashedPassword = HashPassword(password);
                    var newUser = new User
                    {
                        Username = username,
                        PasswordHash = hashedPassword,
                        Email = email,
                        FullName = fullName,
                        RoleID = 2, // Role User (RoleID = 2: User)
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    context.Users.Add(newUser);
                    context.SaveChanges();

                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công", 
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear form
                    ClearRegisterForm();
                    ShowLogin();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng ký: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowRegister_Click(object sender, MouseButtonEventArgs e)
        {
            txtError.Text = "";
            txtError.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
        }

        private void ShowLogin_Click(object sender, MouseButtonEventArgs e)
        {
            ShowLogin();
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Liên hệ admin để cấp lại mật khẩu!", "Quên mật khẩu", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowLogin()
        {
            txtError.Text = "";
            txtError.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
            ClearLoginForm();
        }

        private void ClearLoginForm()
        {
            txtUsername.Text = "";
            txtPassword.Password = "";
        }

        private void ClearRegisterForm()
        {
            txtRegUsername.Text = "";
            txtRegPassword.Password = "";
            txtConfirmPassword.Password = "";
            txtRegEmail.Text = "";
            txtRegFullName.Text = "";
        }
    }
}
