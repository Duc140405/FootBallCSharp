using System;
using System.Linq;
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                txtError.Text = "Vui long nhap day du thong tin!";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password && u.IsActive == true);

                    if (user != null)
                    {
                        var dashboard = new DashboardWindow(user.FullName);
                        dashboard.Show();
                        this.Close();
                        return;
                    }
                    else
                    {
                        txtError.Text = "Sai ten dang nhap hoac mat khau!";
                        txtError.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi ket noi: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtRegUsername.Text.Trim();
            string password = txtRegPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui long nhap day du thong tin!");
                return;
            }

            if (txtRegPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Mat khau khong khop!");
                return;
            }

            try
            {
                using (var db = new FootballDbContext())
                {
                    var existing = db.Users.FirstOrDefault(u => u.Username == username);
                    if (existing != null)
                    {
                        MessageBox.Show("Ten dang nhap da ton tai!");
                        return;
                    }

                    var defaultRole = db.Roles.FirstOrDefault();
                    if (defaultRole == null)
                    {
                        defaultRole = new Role { RoleName = "User" };
                        db.Roles.Add(defaultRole);
                        db.SaveChanges();
                    }

                    var newUser = new User
                    {
                        Username = username,
                        PasswordHash = password,
                        FullName = username,
                        RoleID = defaultRole.RoleID,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Dang ky thanh cong!");
                    ShowLogin();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi dang ky: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show("Lien he admin de cap lai mat khau!");
        }

        private void ShowLogin()
        {
            txtError.Text = "";
            txtError.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
        }
    }
}
