using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels;
using EBISX_POS.ViewModels.Manager;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SkiaSharp;
using System.Diagnostics;


namespace EBISX_POS.Views {

    public partial class DailySalesReportView : Window
    {
        private readonly IConfiguration _configuration;

        public DailySalesReportView(IConfiguration configuration)
        {
            InitializeComponent();
            DataContext = new InvoiceReceiptViewModel();

            var _generateButton = this.FindControl<Button>("GenerateButton");
            _generateButton.Click += GenerateFile;

            _configuration = configuration;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }
        private async void GenerateFile(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Get the folder path from appsettings.json
                string folderPath = _configuration["SalesReport:DailySalesReport"];
                Directory.CreateDirectory(folderPath); // Create if it doesn't exist

                // Define the PDF file path
                string fileName = "DailySalesReport.pdf";
                string filePath = Path.Combine(folderPath, fileName);

                // Get the data from ViewModel
                var viewModel = (InvoiceReceiptViewModel)DataContext;
                var invoice = viewModel.Reports.FirstOrDefault(); // Get the first report

                if (invoice == null)
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Error", "No data available for the report!", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                    return;
                }

                // Create a new PDF document
                using (var stream = new SKFileWStream(filePath))
                using (var document = SKDocument.CreatePdf(stream))
                {
                    var pdfCanvas = document.BeginPage(612, 792); // A4 size in points (8.5x11 inches)

                    using (var paint = new SKPaint())
                    {
                        paint.TextSize = 16;
                        paint.Color = SKColors.Black;
                        paint.IsAntialias = true;
                        paint.Typeface = SKTypeface.FromFamilyName("Arial");

                        float yOffset = 40;

                        void DrawText(string text, float x, float y, float textSize = 14)
                        {
                            paint.TextSize = textSize;
                            pdfCanvas.DrawText(text, x, y, paint);
                        }

                        // Header
                        DrawText("===================================", 50, yOffset);
                        yOffset += 20;
                        DrawText("      Daily Sales Report", 50, yOffset, 18);
                        yOffset += 20;
                        DrawText("===================================", 50, yOffset);
                        yOffset += 30;

                        // Business details
                        DrawText(invoice.BusinessName, 50, yOffset);
                        yOffset += 20;
                        DrawText(invoice.Operator, 50, yOffset);
                        yOffset += 20;
                        DrawText(invoice.Address, 50, yOffset);
                        yOffset += 20;

                        // Report details
                        DrawText($"VAT Reg TIN: {invoice.VATRegTIN}", 50, yOffset);
                        yOffset += 20;
                        DrawText($"MIN: {invoice.MIN}", 50, yOffset);
                        yOffset += 20;
                        DrawText($"Serial Number: {invoice.SerialNumber}", 50, yOffset);
                        yOffset += 20;
                        DrawText($"Report Type: {invoice.ReportType}", 50, yOffset);
                        yOffset += 30;

                        // Sales data
                        DrawText($"Sales For The Day: {invoice.SalesForTheDay:C}", 50, yOffset, 16);
                        yOffset += 20;
                        DrawText($"Vatable Sales: {invoice.VatableSales:C}", 50, yOffset);
                        yOffset += 20;
                        DrawText($"VAT Amount: {invoice.VatAmount:C}", 50, yOffset);
                        yOffset += 20;
                        DrawText($"Cash In Drawer: {invoice.CashInDrawer:C}", 50, yOffset);
                        yOffset += 30;

                        // End of report
                        DrawText("===================================", 50, yOffset);
                        yOffset += 20;
                        DrawText("      End of Report", 50, yOffset, 18);
                        DrawText("===================================", 50, yOffset + 20);

                        // Finish writing to the page
                        document.EndPage();
                        document.Close();
                    }
                }

                // Show success message
                await MessageBoxManager
                    .GetMessageBoxStandard("Success", $"Report saved to {filePath}", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);

                // Open PDF automatically
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (UnauthorizedAccessException)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", "Access Denied! Run the app as Administrator or choose another folder.", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
            }
            catch (Exception ex)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", ex.Message, ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
            }
        }

    }

};