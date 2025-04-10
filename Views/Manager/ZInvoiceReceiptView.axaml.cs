using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels.Manager;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Diagnostics;
using System.IO;
using System;

namespace EBISX_POS.Views {

    public partial class ZInvoiceReceiptView : Window
    {
        public ZInvoiceReceiptView()
        {
            InitializeComponent();
            DataContext = new ZInvoiceReceiptViewModel();
        }

        private async void Print_Button(object? sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = "C:\\Users\\User\\source\\repos";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = "ZInvoiceReceipt.txt";
                string filePath = Path.Combine(folderPath, fileName);

                var viewModel = (ZInvoiceReceiptViewModel)DataContext;

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    int receiptWidth = 50;
                    string CenterText(string text) => text.PadLeft((receiptWidth + text.Length) / 2).PadRight(receiptWidth);

                    writer.WriteLine(new string('=', receiptWidth));
                    writer.WriteLine(CenterText(viewModel.Report.InvoiceType));
                    writer.WriteLine(new string('=', receiptWidth));
                    writer.WriteLine(CenterText(viewModel.Report.Name));
                    writer.WriteLine(CenterText(viewModel.Report.Address));
                    writer.WriteLine(CenterText($"TIN: {viewModel.Report.TIN}"));
                    writer.WriteLine(CenterText($"MIN: {viewModel.Report.MIN}"));
                    writer.WriteLine(CenterText($"MIN: {viewModel.Report.SNumber}"));
                    writer.WriteLine(CenterText($"Store Code: {viewModel.Report.StoreCode}"));
                    writer.WriteLine(CenterText($"{"Report Date: "}{viewModel.Report.StartReportDate}"));
                    writer.WriteLine(CenterText($"{"Report EndDate : "}{viewModel.Report.EndReportDate}"));

                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();

                   

                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();

                    // Write totals
                    writer.WriteLine(CenterText("SALES READING SUMMARY"));
                    writer.WriteLine($"---------", 20);
                    writer.WriteLine(CenterText($"{"Gross Sales",-20}{viewModel.Report.TotalSales,25:C}"));
                    writer.WriteLine(CenterText($"{"Return:",-20}{viewModel.Report.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"Net of return :",-20}{viewModel.Report.TotalReturnAdjustment,25:C}"));
                    writer.WriteLine(CenterText($"{"Less: Discounts :",-20}{viewModel.Report.TotalDiscount,25:C}"));
                    writer.WriteLine($"---------", 20);
                    writer.WriteLine(CenterText($"{"Net Sales:",-20}{viewModel.Report.TotalNetSales,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Vatable :",-20}{viewModel.Report.TotalVATableSales,25:C}"));
                    writer.WriteLine(CenterText($"{"Zero Rated Sales:",-20}{viewModel.Report.TotalZeroRatedSales,25:C}"));
                    writer.WriteLine(CenterText($"{"Vat Exempt Sales:",-20}{viewModel.Report.TotalVATExemptSales,25:C}"));
                    writer.WriteLine(CenterText($"{"12 % VAT :",-20}{viewModel.Report.TotalVATAmount,25:C}"));
                    writer.WriteLine(new string('-', receiptWidth));


                    // Discounts and Adjustments
                    writer.WriteLine();
                    writer.WriteLine(CenterText("SALES ANALYSIS"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Dine In: ",-20}{viewModel.Report.DineIn,25:C}"));
                    writer.WriteLine(CenterText($"{"Take Out: ",-20}{viewModel.Report.TakeOut,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"SC Discount:",-20}{viewModel.Report.SCDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"PWD Discount:",-20}{viewModel.Report.PWDDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"Transact Count:",-20}{viewModel.Report.SCDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"PAX Count:",-20}{viewModel.Report.PaxCount,25:C}"));
                    writer.WriteLine(CenterText($"{"Ave. Per Trnx: ",-20}{viewModel.Report.TotalVoidAdjustment,25:C}"));
                    writer.WriteLine(CenterText($"{"Last O.R. Number: ",-20}{viewModel.Report.LastORNumber,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Total Void :",-20}{viewModel.Report.TotalVoidAdjustment,25:C}"));
                    writer.WriteLine(CenterText($"{"No. of Return Trnx.",-20}{viewModel.Report.TotalReturnAdjustment,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Total Return :",-20}{viewModel.Report.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"No. of Void Trnx.",-20}{viewModel.Report.TotalNumVoid,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"No. of Z Reset",-20}{viewModel.Report.ZReset,25}"));

                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));

                    // Transaction Summary
                    writer.WriteLine(CenterText("TRANSACTION SUMMARY ANALYSIS"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Cashin Drawer:",-20}{viewModel.Report.CashInDrawer,25:C}"));
                    writer.WriteLine(CenterText($"{"Credit Card:",-20}{viewModel.Report.CreditCard,25:C}"));
                    writer.WriteLine(CenterText($"{"Gift Certificate:",-20}{viewModel.Report.GiftCertificate,25:C}"));
                    writer.WriteLine(CenterText($"{"Payment Received:",-20}{viewModel.Report.TotalPaymentsReceived,25:C}"));
                    writer.WriteLine();


                    writer.WriteLine(CenterText("OTHER TRANSACTION ANALYSIS"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Cashier Name:",-20}{viewModel.Report.CashierName,25}"));
                    writer.WriteLine(CenterText($"{"Short/Over:",-20}{viewModel.Report.TotalShortOver,25:C}"));
                    writer.WriteLine(new string('-', receiptWidth));

                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();
                   
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine(CenterText("SALES HISTORY"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Prev Grand Total:",-20}{viewModel.Report.PreviousAccumulatedSales,25}"));
                    writer.WriteLine(CenterText($"{"Gross Sales:",-20}{viewModel.Report.PresentAccumulatedSales,25:C}"));
                    writer.WriteLine(CenterText($"{"Return",-3}{viewModel.Report.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"New Grand Total:",-20}{viewModel.Report.SalesForTheDay,25}"));

                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();
                }

                if (File.Exists(filePath))
                {
                    await MessageBoxManager.GetMessageBoxStandard("Notification", "Printing Receipt...", ButtonEnum.Ok).ShowAsPopupAsync(this);
                }
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (UnauthorizedAccessException)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", "Access Denied! Run the app as Administrator.", ButtonEnum.Ok).ShowAsPopupAsync(this);
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, ButtonEnum.Ok).ShowAsPopupAsync(this);
            }
        }

    }
};
