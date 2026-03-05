using System.Windows;

namespace Football_Management_System
{
    public partial class MatchResultMainWindow : Window
    {
        public MatchResultMainWindow()
        {
            InitializeComponent();
        }

        private void BtnMatchResult_Click(object sender, RoutedEventArgs e)
        {
            MatchResultWindow window = new MatchResultWindow();
            window.ShowDialog();
        }
    }
}
