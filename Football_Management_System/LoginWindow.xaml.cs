using System.Windows;
using System.Windows.Input;

namespace Football_Management_System.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (username == "admin" && password == "123")
            {
                MessageBox.Show("Đăng nhập thành công!");
            }
            else
            {
                txtError.Text = "Sai tên đăng nhập hoặc mật khẩu!";
                txtError.Visibility = Visibility.Visible;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtRegPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Mật khẩu không khớp!");
                return;
            }

            MessageBox.Show("Đăng ký thành công!");
            ShowLogin();
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
            MessageBox.Show("Liên hệ admin để cấp lại mật khẩu!");
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
