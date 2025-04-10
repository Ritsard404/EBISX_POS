using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels;
using Microsoft.Extensions.Options;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.IO;
using System.Linq;

namespace EBISX_POS.Views
{
    public partial class CashTrackView : Window
    {
        private readonly string _cashTrackReportPath;

        public CashTrackView(IOptions<SalesReport> reportOptions)
        {
            InitializeComponent();
            DataContext = new CashTrackerViewModel();

            var _generateButton = this.FindControl<Button>("Print");
            _generateButton.Click += GenerateCashTrack;

            _cashTrackReportPath = reportOptions.Value.CashTrackReport;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void GenerateCashTrack(object? sender, RoutedEventArgs e)
        {
            // Ensure the target directory exists
            if (!Directory.Exists(_cashTrackReportPath))
            {
                Directory.CreateDirectory(_cashTrackReportPath);
            }

            // Define the file path
            string fileName = "CashTrackReport.txt";
            string filePath = Path.Combine(_cashTrackReportPath, fileName);

            var viewModel = (CashTrackerViewModel)DataContext;
            var cashTrack = viewModel.CashTrack.FirstOrDefault();

            if (cashTrack == null)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", "No data available for the report!", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
                return;
            }

            string reportContent = $@"
                ==================================
                        Cash Track Report
                ==================================
                Cash In Drawer: {cashTrack.CashInDrawer:C}
                Total Cash Drawer: {cashTrack.WithdrawalAmount:C}
                ";

            reportContent = string.Join("\n", reportContent.Split("\n").Select(line => line.Trim()));
            File.WriteAllText(filePath, reportContent);
        }
    }
}
