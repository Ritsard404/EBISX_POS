using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels;
using Microsoft.Extensions.Configuration;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;

namespace EBISX_POS.Views {
    public partial class CashTrackView : Window
    {
        private readonly IConfiguration _configuration;

        // Parameterless constructor for XAML instantiation
        public CashTrackView(IConfiguration configuration)
        {
            InitializeComponent();
            DataContext = new CashTrackerViewModel();

            var _generateButton = this.FindControl<Button>("Print");
            _generateButton.Click += GenerateCashTrack;

            _configuration = configuration;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }

        public async void GenerateCashTrack(object? sender, RoutedEventArgs e)
        {
                // Define the folder path
                string folderPath = _configuration["SalesReport:CashTrackReport"];
                Directory.CreateDirectory(folderPath); // Create if it doesn't exist

                // Define the file path
                string fileName = "CashTrackReport.txt";
                string filePath = Path.Combine(folderPath, fileName);

                // Get the data from ViewModel
                var viewModel = (CashTrackerViewModel)DataContext;
                var cashTrack = viewModel.CashTrack.FirstOrDefault(); // Get the first report

                if (cashTrack == null)
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Error", "No data available for the report!", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                    return;
                }

                // Format the report content
                string reportContent = $@"
                ==================================
                        Cash Track Report
                ==================================
                Cash In Drawer: {cashTrack.CashInDrawer:C}
                Total Cash Drawer: {cashTrack.WithdrawalAmount:C}
                ";

                // Remove extra spaces at the start of each line
                reportContent = string.Join("\n", reportContent.Split("\n").Select(line => line.Trim()));

                // Write to the file
                File.WriteAllText(filePath, reportContent);
        }
    }

};
