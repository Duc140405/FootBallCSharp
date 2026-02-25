using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClosedXML.Excel;
using Microsoft.Win32;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Football_Management_System
{
    public partial class ReportWindow : Window
    {
        FootballTournamentEntities db = new FootballTournamentEntities();

        public ReportWindow()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            txtTeams.Text = db.Teams.Count().ToString();
            txtPlayers.Text = db.Players.Count().ToString();
            txtMatches.Text = db.Matches.Count().ToString();

            var data = db.Teams.Select(t => new
            {
                Team = t.TeamName,
                Players = t.Players.Count,
                Matches = db.Matches.Count(m => m.HomeTeamId == t.TeamId || m.AwayTeamId == t.TeamId)
            }).ToList();

            gridStats.ItemsSource = data;
        }

        // ===== EXCEL =====
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Excel|*.xlsx";

            if (s.ShowDialog() == true)
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Report");

                ws.Cell(1, 1).Value = "Team";
                ws.Cell(1, 2).Value = "Players";
                ws.Cell(1, 3).Value = "Matches";

                int row = 2;
                foreach (dynamic item in gridStats.ItemsSource)
                {
                    ws.Cell(row, 1).Value = item.Team;
                    ws.Cell(row, 2).Value = item.Players;
                    ws.Cell(row, 3).Value = item.Matches;
                    row++;
                }

                wb.SaveAs(s.FileName);
                MessageBox.Show("Xuất Excel OK");
            }
        }

        // ===== PDF =====
        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "PDF|*.pdf";

            if (s.ShowDialog() == true)
            {
                Document doc = new Document();
                PdfWriter.GetInstance(doc, new System.IO.FileStream(s.FileName, System.IO.FileMode.Create));

                doc.Open();
                doc.Add(new Paragraph("Bao cao giai dau"));

                foreach (dynamic item in gridStats.ItemsSource)
                {
                    doc.Add(new Paragraph($"{item.Team} - {item.Players} - {item.Matches}"));
                }

                doc.Close();
                MessageBox.Show("Xuất PDF OK");
            }
        }

        // ===== PRINT =====
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