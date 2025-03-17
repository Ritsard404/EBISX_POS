using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using EBISX_POS.ViewModels.Manager;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls.Shapes;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EBISX_POS.Views
{
    public partial class TransactionView : Window
    {
        private readonly IConfiguration _configuration;

        public TransactionView(IConfiguration configuration)
        {
            InitializeComponent();
            DataContext = new TransactionViewModel();
            _configuration = configuration;

        }

        // This method handles the Print Tlogs button click event
        private async void Print_Tlogs(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                // Get the folder path from appsettings.json
                string folderPath = _configuration["SalesReport:TransactionLogs"];
                Directory.CreateDirectory(folderPath); // Create if it doesn't exist

                // Define the file path
                string fileName = "TransactionReceipt.txt";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                var viewModel = (TransactionViewModel)DataContext;
                var selectedTransaction = viewModel.SelectedTransactionLog;

                // is order are function
                Debug.WriteLine($"Selected Transaction: {selectedTransaction?.TransactionId}");

                if (selectedTransaction == null)
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Error", "No transaction selected!", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                    return;
                }

                // Format the receipt content
                string receiptContent = $@"
                ======================================
                        Transaction Receipt
                ======================================
                 
                Transaction ID: {selectedTransaction?.TransactionId ?? "N/A"}
                Date: {selectedTransaction?.LogDate.ToString("MM/dd/yyyy") ?? "N/A"}
                Time: {selectedTransaction?.LogTime.ToString(@"hh\:mm\:ss") ?? "N/A"}
                Total Amount: {selectedTransaction?.TotalAmount.ToString("C") ?? "N/A"}
                Action: {selectedTransaction?.Action ?? "N/A"}
                Details: {selectedTransaction?.Details ?? "N/A"}
                Crew Member: {selectedTransaction?.CrewMember?.Name ?? "N/A"}
                
                ======================================";

                // Remove extra spaces at the start of each line
                receiptContent = string.Join("\n", receiptContent.Split("\n").Select(line => line.Trim()));

                // Write to the file
                File.WriteAllText(filePath, receiptContent);

                // Show success message
                await MessageBoxManager
                    .GetMessageBoxStandard("Success", $"Receipt saved to {filePath}", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
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
}