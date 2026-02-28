using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
// TODO: Cần cài NuGet packages nếu chưa có:
//   Install-Package ClosedXML
//   Install-Package iTextSharp
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using Football_Management_System.DataAccess;
using System.IO;
using System.Collections;

namespace Football_Management_System
{
    public partial class ReportWindow : Window
    {
        // Database helper
        private readonly DatabaseHelper db = new DatabaseHelper();

        public ReportWindow()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            try
            {
                // tổng quan
                txtTeams.Text = db.GetTeamsCount().ToString();
                txtPlayers.Text = db.GetPlayersCount().ToString();

                var matches = db.GetAllMatches();
                txtMatches.Text = matches?.Count.ToString() ?? "0";

                // chi tiết theo đội
                var data = db.GetTeamStats();
                gridStats.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== EXCEL =====
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Excel|*.xlsx";

            if (s.ShowDialog() != true)
                return;

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Report");

                    ws.Cell(1, 1).Value = "Team";
                    ws.Cell(1, 2).Value = "Players";
                    ws.Cell(1, 3).Value = "Matches";

                    int row = 2;
                    var items = gridStats.ItemsSource as IEnumerable;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            try
                            {
                                dynamic d = it;
                                ws.Cell(row, 1).Value = d.Team;
                                ws.Cell(row, 2).Value = d.Players;
                                ws.Cell(row, 3).Value = d.Matches;
                            }
                            catch
                            {
                                // fallback
                                ws.Cell(row, 1).Value = it?.ToString();
                            }
                            row++;
                        }
                    }

                    ws.Columns().AdjustToContents();
                    wb.SaveAs(s.FileName);
                }

                MessageBox.Show("Xuất Excel (.xlsx) thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== PDF =====
        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "PDF|*.pdf";

            if (s.ShowDialog() != true)
                return;

            try
            {
                using (var fs = new FileStream(s.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                    doc.Add(new Paragraph("BÁO CÁO GIẢI ĐẤU", titleFont));
                    doc.Add(new Paragraph(" "));

                    var items = gridStats.ItemsSource as IEnumerable;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            try
                            {
                                dynamic d = it;
                                string line = string.Empty;
                                try { line = $"{d.Team} - {d.Players} cầu thủ - {d.Matches} trận"; }
                                catch { line = d?.ToString() ?? string.Empty; }

                                doc.Add(new Paragraph(line, normalFont));
                            }
                            catch { }
                        }
                    }

                    doc.Close();
                }

                MessageBox.Show("Xuất PDF thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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