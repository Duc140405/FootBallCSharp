using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Football_Management_System.DataAccess;

namespace Football_Management_System
{
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    txtTeams.Text = db.Teams.Count().ToString();
                    txtPlayers.Text = db.Players.Count().ToString();
                    txtMatches.Text = db.Matches.Count().ToString();

                    var data = db.Teams.Select(t => new
                    {
                        Team = t.TeamName,
                        Players = t.Players.Count(),
                        Matches = t.HomeMatches.Count() + t.AwayMatches.Count()
                    }).ToList();

                    gridStats.ItemsSource = data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi load du lieu: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chuc nang xuat Excel can cai them NuGet ClosedXML.", "Thong bao");
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chuc nang xuat PDF can cai them NuGet iTextSharp.", "Thong bao");
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog p = new PrintDialog();
            if (p.ShowDialog() == true)
            {
                p.PrintVisual(gridStats, "Report");
            }
        }
    }
}