using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Win32; // Dùng cho SaveFileDialog
using Football_Management_System.DataAccess;
using ClosedXML.Excel; // Thư viện Excel
using iText.Kernel.Pdf; // Thư viện PDF
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.Windows.Controls;

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
                MessageBox.Show("Lỗi khi load dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- CHỨC NĂNG XUẤT EXCEL ---
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var data = gridStats.ItemsSource as IEnumerable<dynamic>;
            if (data == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "BaoCaoBongDa.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Thống kê");

                        // Header
                        worksheet.Cell(1, 1).Value = "Tên Đội";
                        worksheet.Cell(1, 2).Value = "Số Cầu Thủ";
                        worksheet.Cell(1, 3).Value = "Số Trận Đã Đấu";

                        // Data
                        int row = 2;
                        foreach (var item in data)
                        {
                            worksheet.Cell(row, 1).Value = item.Team;
                            worksheet.Cell(row, 2).Value = item.Players;
                            worksheet.Cell(row, 3).Value = item.Matches;
                            row++;
                        }

                        // Format bảng
                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                    MessageBox.Show("Xuất Excel thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        // --- CHỨC NĂNG XUẤT PDF ---
        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            var data = gridStats.ItemsSource as IEnumerable<dynamic>;
            if (data == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Document|*.pdf",
                FileName = "BaoCaoBongDa.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (PdfWriter writer = new PdfWriter(saveFileDialog.FileName))
                    {
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            Document document = new Document(pdf);

                            // Tiêu đề
                            document.Add(new Paragraph("BAO CAO THONG KE DOI BONG").SetFontSize(20));
                            document.Add(new Paragraph($"Ngay xuat: {DateTime.Now:dd/MM/yyyy HH:mm}"));
                            document.Add(new Paragraph("\n"));

                            // Tạo bảng (3 cột)
                            Table table = new Table(3);
                            table.AddHeaderCell("Ten Doi");
                            table.AddHeaderCell("So Cau Thu");
                            table.AddHeaderCell("So Tran");

                            foreach (var item in data)
                            {
                                table.AddCell(item.Team.ToString());
                                table.AddCell(item.Players.ToString());
                                table.AddCell(item.Matches.ToString());
                            }

                            document.Add(table);
                        }
                    }
                    MessageBox.Show("Xuất PDF thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message);
                }
            }
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