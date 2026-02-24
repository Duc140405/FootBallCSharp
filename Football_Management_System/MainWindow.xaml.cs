using System.Windows;

namespace Football_Management_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnQuanLyGiaiDau_Click(object sender, RoutedEventArgs e)
        {
            QuanLyGiaiDau window = new QuanLyGiaiDau();
            window.ShowDialog();
        }

        private void BtnQuanLyDoiBong_Click(object sender, RoutedEventArgs e)
        {
            QuanLyDoiBong window = new QuanLyDoiBong();
            window.ShowDialog();
        }

        private void BtnKetQuaTranDau_Click(object sender, RoutedEventArgs e)
        {
            MatchResultWindow window = new MatchResultWindow();
            window.ShowDialog();
        }

        private void BtnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            BaoCaoXuatDuLieu window = new BaoCaoXuatDuLieu();
            window.ShowDialog();
        }
    }
}
