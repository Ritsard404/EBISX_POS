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

                // Define the file path
                string fileName = "DailySalesReport.txt";
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

                // Format the report content
                string reportContent = $@"
            ==================================
                    Daily Sales Report
            ==================================
                  {invoice.BusinessName}
                   {invoice.Operator}
                   {invoice.Address}
            VAT Reg TIN: {invoice.VATRegTIN}
            MIN: {invoice.MIN}
            Serial Number: {invoice.SerialNumber}
            Report Type: {invoice.ReportType}
            -------------------------------------
            Report Date: {invoice.ReportDate}
            Report Time: {invoice.ReportTime}
            Start Date/Time: {invoice.StartDateTime}
            End Date/Time: {invoice.EndDateTime}
            Cashier: {invoice.Cashier}
            ----------------------------
            Beginning SI: {invoice.BeginningSI}
            Ending SI: {invoice.EndingSI}
            Beginning VOID: {invoice.BeginningVOID}
            Ending VOID: {invoice.EndingVOID}
            ----------------------------
            Opening Fund: {invoice.OpeningFund:C}
            Cash Received: {invoice.CashReceived:C}
            Cheque Received: {invoice.ChequeReceived:C}
            Credit Card Received: {invoice.CreditCardReceived:C}
            Total Payments: {invoice.TotalPayments:C}
            ----------------------------
            Present Accumulated Sales: {invoice.PresentAccumulatedSales:C}
            Previous Accumulated Sales: {invoice.PreviousAccumulatedSales:C}
            Sales For The Day: {invoice.SalesForTheDay:C}
            ----------------------------
            Zero Rated Sales: {invoice.ZeroRatedSales:C}
            VAT Exempt Sales: {invoice.VatExemptSales:C}
            Vatable Sales: {invoice.VatableSales:C}
            VAT Amount: {invoice.VatAmount:C}
            ----------------------------
            Cash In Drawer: {invoice.CashInDrawer:C}
            Withdrawal Amount: {invoice.WithdrawalAmount:C}
            Short/Over: {invoice.ShortOver:C}
            ----------------------------

        ";

                // Remove extra spaces at the start of each line
                reportContent = string.Join("\n", reportContent.Split("\n").Select(line => line.Trim()));

                // Write to the file
                File.WriteAllText(filePath, reportContent);

                // Show success message
                await MessageBoxManager
                    .GetMessageBoxStandard("Success", $"Report saved to {filePath}", ButtonEnum.Ok)
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

};