using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Football_Management_System.DataAccess;
using Football_Management_System.Models;
using Football_Management_System.Statistics.Models;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Football_Management_System
{
    public partial class StatisticsWindow : Window
    {
        private List<PlayerStatsEntry> currentPlayerGroups = new List<PlayerStatsEntry>();
        private string currentTournamentName = "Tat ca";

        public StatisticsWindow()
        {
            InitializeComponent();
            LoadTournaments();
            LoadData();
        }

        private void LoadTournaments()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    var list = db.Tournaments.OrderBy(t => t.TournamentName).ToList();
                    list.Insert(0, new Tournament { TournamentID = 0, TournamentName = "-- Tat ca --" });
                    cboTournament.ItemsSource = list;
                    cboTournament.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tai giai dau: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var db = new FootballDbContext())
                {
                    int tournamentId = 0;
                    if (cboTournament.SelectedValue != null)
                        tournamentId = (int)cboTournament.SelectedValue;

                    var selectedTournament = cboTournament.SelectedItem as Tournament;
                    currentTournamentName = selectedTournament != null ? selectedTournament.TournamentName : "Tat ca";

                    var statsQuery = db.PlayerStatistics
                        .Include(ps => ps.Player)
                        .Include(ps => ps.Player.Team)
                        .AsQueryable();

                    if (tournamentId > 0)
                        statsQuery = statsQuery.Where(ps => ps.TournamentID == tournamentId);

                    var statsList = statsQuery.ToList();

                    int totalMatches = 0;
                    if (tournamentId > 0)
                        totalMatches = db.Matches.Count(m => m.TournamentID == tournamentId);
                    else
                        totalMatches = db.Matches.Count();

                    int totalGoals = statsList.Sum(ps => ps.Goals ?? 0);
                    int totalYellow = statsList.Sum(ps => ps.YellowCards ?? 0);
                    int totalRed = statsList.Sum(ps => ps.RedCards ?? 0);

                    txtTotalMatches.Text = totalMatches.ToString();
                    txtTotalGoals.Text = totalGoals.ToString();
                    txtTotalYellow.Text = totalYellow.ToString();
                    txtTotalRed.Text = totalRed.ToString();

                    currentPlayerGroups = statsList
                        .GroupBy(ps => new { ps.PlayerID, ps.Player.PlayerName, TeamName = ps.Player.Team != null ? ps.Player.Team.TeamName : "", ps.Player.Position })
                        .Select(g => new PlayerStatsEntry
                        {
                            PlayerName = g.Key.PlayerName,
                            TeamName = g.Key.TeamName,
                            Position = g.Key.Position,
                            Goals = g.Sum(x => x.Goals ?? 0),
                            Assists = g.Sum(x => x.Assists ?? 0),
                            YellowCards = g.Sum(x => x.YellowCards ?? 0),
                            RedCards = g.Sum(x => x.RedCards ?? 0),
                            MinutesPlayed = g.Sum(x => x.MinutesPlayed ?? 0)
                        }).ToList();

                    dgTopScorers.ItemsSource = currentPlayerGroups
                        .OrderByDescending(p => p.Goals)
                        .Take(10).ToList();

                    dgTopAssists.ItemsSource = currentPlayerGroups
                        .OrderByDescending(p => p.Assists)
                        .Take(10).ToList();

                    dgTopCards.ItemsSource = currentPlayerGroups
                        .OrderByDescending(p => p.YellowCards + p.RedCards)
                        .Take(10).ToList();

                    dgBestXI.ItemsSource = currentPlayerGroups
                        .OrderByDescending(p => p.MinutesPlayed)
                        .Take(11).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tai thong ke: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cboTournament_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                LoadData();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // === EXPORT GIAI THUONG PDF ===
        private void ExportAwardsPdf_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayerGroups == null || currentPlayerGroups.Count == 0)
            {
                MessageBox.Show("Khong co du lieu de xuat!", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "PDF Document|*.pdf",
                FileName = "GiaiThuong_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf"
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                var topScorers = currentPlayerGroups.OrderByDescending(p => p.Goals).Take(3).ToList();
                var topAssists = currentPlayerGroups.OrderByDescending(p => p.Assists).Take(3).ToList();
                var topCards = currentPlayerGroups.OrderByDescending(p => p.YellowCards + p.RedCards).Take(3).ToList();
                var bestXI = currentPlayerGroups.OrderByDescending(p => p.MinutesPlayed).Take(11).ToList();

                using (var writer = new PdfWriter(dlg.FileName))
                using (var pdf = new PdfDocument(writer))
                {
                    var doc = new Document(pdf);
                    var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    doc.Add(new Paragraph("GIAI THUONG - THONG KE GIAI DAU").SetFontSize(22).SetFont(boldFont));
                    doc.Add(new Paragraph("Giai dau: " + currentTournamentName).SetFontSize(14));
                    doc.Add(new Paragraph("Ngay xuat: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SetFontSize(11));
                    doc.Add(new Paragraph("Tong so tran: " + txtTotalMatches.Text +
                        " | Tong ban thang: " + txtTotalGoals.Text +
                        " | The vang: " + txtTotalYellow.Text +
                        " | The do: " + txtTotalRed.Text).SetFontSize(11));
                    doc.Add(new Paragraph("\n"));

                    // Top Ghi Ban
                    doc.Add(new Paragraph("TOP GHI BAN").SetFontSize(16).SetFont(boldFont));
                    var tblScorers = new Table(UnitValue.CreatePercentArray(new float[] { 10, 40, 30, 20 })).UseAllAvailableWidth();
                    tblScorers.AddHeaderCell("Hang");
                    tblScorers.AddHeaderCell("Cau thu");
                    tblScorers.AddHeaderCell("Doi");
                    tblScorers.AddHeaderCell("Ban thang");
                    for (int i = 0; i < topScorers.Count; i++)
                    {
                        string medal = i == 0 ? "1st" : i == 1 ? "2nd" : "3rd";
                        tblScorers.AddCell(medal);
                        tblScorers.AddCell(topScorers[i].PlayerName);
                        tblScorers.AddCell(topScorers[i].TeamName);
                        tblScorers.AddCell(topScorers[i].Goals.ToString());
                    }
                    doc.Add(tblScorers);
                    doc.Add(new Paragraph("\n"));

                    // Top Kien Tao
                    doc.Add(new Paragraph("TOP KIEN TAO").SetFontSize(16).SetFont(boldFont));
                    var tblAssists = new Table(UnitValue.CreatePercentArray(new float[] { 10, 40, 30, 20 })).UseAllAvailableWidth();
                    tblAssists.AddHeaderCell("Hang");
                    tblAssists.AddHeaderCell("Cau thu");
                    tblAssists.AddHeaderCell("Doi");
                    tblAssists.AddHeaderCell("Kien tao");
                    for (int i = 0; i < topAssists.Count; i++)
                    {
                        string medal = i == 0 ? "1st" : i == 1 ? "2nd" : "3rd";
                        tblAssists.AddCell(medal);
                        tblAssists.AddCell(topAssists[i].PlayerName);
                        tblAssists.AddCell(topAssists[i].TeamName);
                        tblAssists.AddCell(topAssists[i].Assists.ToString());
                    }
                    doc.Add(tblAssists);
                    doc.Add(new Paragraph("\n"));

                    // Top The Phat
                    doc.Add(new Paragraph("TOP THE PHAT").SetFontSize(16).SetFont(boldFont));
                    var tblCards = new Table(UnitValue.CreatePercentArray(new float[] { 10, 35, 25, 15, 15 })).UseAllAvailableWidth();
                    tblCards.AddHeaderCell("Hang");
                    tblCards.AddHeaderCell("Cau thu");
                    tblCards.AddHeaderCell("Doi");
                    tblCards.AddHeaderCell("Vang");
                    tblCards.AddHeaderCell("Do");
                    for (int i = 0; i < topCards.Count; i++)
                    {
                        string medal = i == 0 ? "1st" : i == 1 ? "2nd" : "3rd";
                        tblCards.AddCell(medal);
                        tblCards.AddCell(topCards[i].PlayerName);
                        tblCards.AddCell(topCards[i].TeamName);
                        tblCards.AddCell(topCards[i].YellowCards.ToString());
                        tblCards.AddCell(topCards[i].RedCards.ToString());
                    }
                    doc.Add(tblCards);
                    doc.Add(new Paragraph("\n"));

                    // Doi Hinh Tieu Bieu
                    doc.Add(new Paragraph("DOI HINH TIEU BIEU").SetFontSize(16).SetFont(boldFont));
                    var tblBest = new Table(UnitValue.CreatePercentArray(new float[] { 8, 35, 15, 25, 17 })).UseAllAvailableWidth();
                    tblBest.AddHeaderCell("#");
                    tblBest.AddHeaderCell("Cau thu");
                    tblBest.AddHeaderCell("Vi tri");
                    tblBest.AddHeaderCell("Doi");
                    tblBest.AddHeaderCell("Phut");
                    for (int i = 0; i < bestXI.Count; i++)
                    {
                        tblBest.AddCell((i + 1).ToString());
                        tblBest.AddCell(bestXI[i].PlayerName);
                        tblBest.AddCell(bestXI[i].Position ?? "");
                        tblBest.AddCell(bestXI[i].TeamName);
                        tblBest.AddCell(bestXI[i].MinutesPlayed.ToString());
                    }
                    doc.Add(tblBest);
                }

                MessageBox.Show("Xuat PDF giai thuong thanh cong!\n" + dlg.FileName, "Thanh cong", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi xuat PDF: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // === EXPORT GIAI THUONG EXCEL ===
        private void ExportAwardsExcel_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayerGroups == null || currentPlayerGroups.Count == 0)
            {
                MessageBox.Show("Khong co du lieu de xuat!", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "GiaiThuong_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                var topScorers = currentPlayerGroups.OrderByDescending(p => p.Goals).Take(3).ToList();
                var topAssists = currentPlayerGroups.OrderByDescending(p => p.Assists).Take(3).ToList();
                var topCards = currentPlayerGroups.OrderByDescending(p => p.YellowCards + p.RedCards).Take(3).ToList();
                var bestXI = currentPlayerGroups.OrderByDescending(p => p.MinutesPlayed).Take(11).ToList();

                using (var workbook = new XLWorkbook())
                {
                    // Sheet 1: Top Ghi Ban
                    var ws1 = workbook.Worksheets.Add("Top Ghi Ban");
                    ws1.Cell(1, 1).Value = "GIAI THUONG - " + currentTournamentName;
                    ws1.Range("A1:D1").Merge().Style.Font.SetBold(true).Font.SetFontSize(16);
                    ws1.Cell(2, 1).Value = "Ngay xuat: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    ws1.Cell(4, 1).Value = "Hang";
                    ws1.Cell(4, 2).Value = "Cau thu";
                    ws1.Cell(4, 3).Value = "Doi";
                    ws1.Cell(4, 4).Value = "Ban thang";
                    ws1.Range("A4:D4").Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightBlue);
                    for (int i = 0; i < topScorers.Count; i++)
                    {
                        string medal = i == 0 ? "1st" : i == 1 ? "2nd" : "3rd";
                        ws1.Cell(5 + i, 1).Value = medal;
                        ws1.Cell(5 + i, 2).Value = topScorers[i].PlayerName;
                        ws1.Cell(5 + i, 3).Value = topScorers[i].TeamName;
                        ws1.Cell(5 + i, 4).Value = topScorers[i].Goals;
                    }
                    ws1.Columns().AdjustToContents();

                    // Sheet 2: Top Kien Tao
                    var ws2 = workbook.Worksheets.Add("Top Kien Tao");
                    ws2.Cell(1, 1).Value = "Hang";
                    ws2.Cell(1, 2).Value = "Cau thu";
                    ws2.Cell(1, 3).Value = "Doi";
                    ws2.Cell(1, 4).Value = "Kien tao";
                    ws2.Range("A1:D1").Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightGreen);
                    for (int i = 0; i < topAssists.Count; i++)
                    {
                        string medal = i == 0 ? "1st" : i == 1 ? "2nd" : "3rd";
                        ws2.Cell(2 + i, 1).Value = medal;
                        ws2.Cell(2 + i, 2).Value = topAssists[i].PlayerName;
                        ws2.Cell(2 + i, 3).Value = topAssists[i].TeamName;
                        ws2.Cell(2 + i, 4).Value = topAssists[i].Assists;
                    }
                    ws2.Columns().AdjustToContents();

                    // Sheet 3: Top The Phat
                    var ws3 = workbook.Worksheets.Add("Top The Phat");
                    ws3.Cell(1, 1).Value = "Hang";
                    ws3.Cell(1, 2).Value = "Cau thu";
                    ws3.Cell(1, 3).Value = "Doi";
                    ws3.Cell(1, 4).Value = "The vang";
                    ws3.Cell(1, 5).Value = "The do";
                    ws3.Range("A1:E1").Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightYellow);
                    for (int i = 0; i < topCards.Count; i++)
                    {
                        string medal = i == 0 ? "1st" : i == 1 ? "2nd" : "3rd";
                        ws3.Cell(2 + i, 1).Value = medal;
                        ws3.Cell(2 + i, 2).Value = topCards[i].PlayerName;
                        ws3.Cell(2 + i, 3).Value = topCards[i].TeamName;
                        ws3.Cell(2 + i, 4).Value = topCards[i].YellowCards;
                        ws3.Cell(2 + i, 5).Value = topCards[i].RedCards;
                    }
                    ws3.Columns().AdjustToContents();

                    // Sheet 4: Doi Hinh Tieu Bieu
                    var ws4 = workbook.Worksheets.Add("Doi Hinh Tieu Bieu");
                    ws4.Cell(1, 1).Value = "#";
                    ws4.Cell(1, 2).Value = "Cau thu";
                    ws4.Cell(1, 3).Value = "Vi tri";
                    ws4.Cell(1, 4).Value = "Doi";
                    ws4.Cell(1, 5).Value = "Phut thi dau";
                    ws4.Range("A1:E1").Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightCoral);
                    for (int i = 0; i < bestXI.Count; i++)
                    {
                        ws4.Cell(2 + i, 1).Value = i + 1;
                        ws4.Cell(2 + i, 2).Value = bestXI[i].PlayerName;
                        ws4.Cell(2 + i, 3).Value = bestXI[i].Position ?? "";
                        ws4.Cell(2 + i, 4).Value = bestXI[i].TeamName;
                        ws4.Cell(2 + i, 5).Value = bestXI[i].MinutesPlayed;
                    }
                    ws4.Columns().AdjustToContents();

                    workbook.SaveAs(dlg.FileName);
                }

                MessageBox.Show("Xuat Excel giai thuong thanh cong!\n" + dlg.FileName, "Thanh cong", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi xuat Excel: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // === IN GIAI THUONG ===
        private void PrintAwards_Click(object sender, RoutedEventArgs e)
        {
            var printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
            {
                printDlg.PrintVisual(this, "Thong Ke Giai Dau - Giai Thuong");
                MessageBox.Show("Da gui lenh in!", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
