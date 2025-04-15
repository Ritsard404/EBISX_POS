using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels.Manager;
using Microsoft.Extensions.Configuration;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Diagnostics;
using System.IO;

namespace EBISX_POS.Views.Manager {

    public partial class SalesHistoryWindow : Window
    {
        private readonly IConfiguration _configuration;
        //public SalesHistoryWindow()
        public SalesHistoryWindow(IConfiguration configuration)
        {

            InitializeComponent();
            DataContext = new SalesHistoryViewModel();
            _configuration = configuration;

            //// Assuming you have a DatePicker named "datePicker" in your XAML
            var datePicker = this.FindControl<DatePicker>("datePicker");
            datePicker.SelectedDate = new DateTime(2000, 1, 1);

            // Subscribe to PropertyChanged to detect changes to SelectedDate.
            datePicker.PropertyChanged += DatePicker_PropertyChanged;

        }

        private void DatePicker_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            // Check if the SelectedDate property changed.
            if (e.Property == DatePicker.SelectedDateProperty)
            {
                var datePicker = (DatePicker)sender;
                Debug.WriteLine($"[Debug] DatePicker SelectedDate changed to: {datePicker.SelectedDate?.ToString("MM/dd/yyyy")}");
            }
        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                string folderPath = _configuration["SalesReport:TransactionLogs"];
                //string folderPath = "@C:\\EBISX_POS\\SalesReport\\TransactionLogs";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = "SelectedSalesReceipt.txt";
                string filePath = Path.Combine(folderPath, fileName);

                var viewModel = DataContext as SalesHistoryViewModel;
                var selectedSales = viewModel.SelectedRecord;

                if (selectedSales == null)
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Error", "No transaction selected!", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                    return;
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    int receiptWidth = 50;
                    string CenterText(string text) => text.PadLeft((receiptWidth + text.Length) / 2).PadRight(receiptWidth);

                    writer.WriteLine(new string('=', receiptWidth));
                    writer.WriteLine(CenterText(selectedSales.InvoiceType ?? "INVOICE"));
                    writer.WriteLine(new string('=', receiptWidth));
                    writer.WriteLine(CenterText(selectedSales.Name));
                    writer.WriteLine(CenterText(selectedSales.Address));
                    writer.WriteLine(CenterText($"TIN: {selectedSales.TIN}"));
                    writer.WriteLine(CenterText($"MIN: {selectedSales.MIN}"));
                    writer.WriteLine(CenterText($"S/N: {selectedSales.SNumber}"));
                    writer.WriteLine(CenterText($"Store Code: {selectedSales.StoreCode}"));
                    writer.WriteLine(CenterText($"Date: {selectedSales.StartTime}{selectedSales.EndTime}"));
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();

                    writer.WriteLine(CenterText($"{"Beg. SI #:",-20}{selectedSales.BegSI,25}"));
                    writer.WriteLine(CenterText($"{"End SI #:",-20}{selectedSales.EndSI,25}"));
                    writer.WriteLine(CenterText($"{"Beg. Void #:",-20}{selectedSales.BegVOID,25}"));
                    writer.WriteLine(CenterText($"{"End. Void #:",-20}{selectedSales.EndVOID,25}"));
                    writer.WriteLine(CenterText($"{"Beg. Return #:",-20}{selectedSales.BegRETURN,25}"));
                    writer.WriteLine(CenterText($"{"End. Return #:",-20}{selectedSales.EndRETURN,25}"));
                    writer.WriteLine();

                    writer.WriteLine(CenterText($"{"Reset Counter No.",-20} {selectedSales.ZReset,25}"));
                    writer.WriteLine(CenterText($"{"Z Counter No.", -20} {selectedSales.ZReset,25}"));
                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Present Accumulate Sales:",-20}{selectedSales.PresentAccumulatedSales,25:C}"));
                    writer.WriteLine(CenterText($"{"Previous Accumulate Sales:",-20}{selectedSales.PreviousAccumulatedSales,25:C}"));
                    writer.WriteLine(CenterText($"{"Sales of the Day:",-20}{selectedSales.SalesForTheDay,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine(CenterText($"BREAKDOWN OF SALES"));
                    writer.WriteLine(CenterText($"{"VATABLE SALES :",-20}{selectedSales.TotalVATableSales,25:C}"));
                    writer.WriteLine(CenterText($"{"VAT Amount :",-20}{selectedSales.TotalVATableSales,25:C}"));
                    writer.WriteLine(CenterText($"{"ZERO RATED SALES:",-20}{selectedSales.TotalZeroRatedSales,25:C}"));
                    writer.WriteLine(CenterText($"{"VAT EXEMPT SALES:",-20}{selectedSales.TotalVATExemptSales,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine(CenterText($"{"Gross Amount",-20}{selectedSales.TotalSales,25:C}"));
                    writer.WriteLine(CenterText($"{"Less Discount:",-20}{selectedSales.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"Lest Return:",-20}{selectedSales.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"Less Void:",-20}{selectedSales.TotalNumVoid,25:C}"));
                    writer.WriteLine(CenterText($"{"Less VAT Adjustment:",-20}{selectedSales.TotalDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"Net Amount:",-20}{selectedSales.TotalNetSales,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));

                    // Discounts and Adjustments
                    writer.WriteLine();
                    writer.WriteLine(CenterText("DISCOUNT SUMMARY"));           
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"SC Discount:",-20}{selectedSales.SCDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"PWD Discount:",-20}{selectedSales.PWDDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"NAAC Discount:",-20}{selectedSales.SCDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"Solo Parent Disc. :",-20}{selectedSales.TotalVoidAdjustment,25:C}"));
                    writer.WriteLine(CenterText($"{"Other Disc. :",-20}{selectedSales.LastORNumber,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText("SALES ADJUSTMENT"));
                    writer.WriteLine(CenterText($"{"VOID :",-20}{selectedSales.TotalVoidAdjustment,25:C}"));
                    writer.WriteLine(CenterText($"{"RETURN :",-20}{selectedSales.TotalReturnAdjustment,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText("VAT ADJUSTMENT"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"SC Discount:",-20}{selectedSales.SCDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"PWD Discount:",-20}{selectedSales.PWDDiscount,25:C}"));
                    writer.WriteLine(CenterText($"{"REG. Disc TRANS:",-20}{selectedSales.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"ZERO-RATED TRANS:",-20}{selectedSales.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"VAT on Return:",-20}{selectedSales.TotalReturn,25:C}"));
                    writer.WriteLine(CenterText($"{"Other VAT Adjustment:",-20}{selectedSales.TotalReturn,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText("TRANSACTION SUMMARY"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Cashin Drawer:",-20}{selectedSales.CashInDrawer,25:C}"));
                    writer.WriteLine(CenterText($"{"CHEQUE:",-20}{selectedSales.CashInDrawer,25:C}"));

                    writer.WriteLine(CenterText($"{"CREDIT CARD:",-20}{selectedSales.CreditCard,25:C}"));
                    writer.WriteLine(CenterText($"{"GIFT CERTIFICATE:",-20}{selectedSales.GiftCertificate,25:C}"));
                    writer.WriteLine(CenterText($"{"Opening Fund:",-20}{selectedSales.TotalPaymentsReceived,25:C}"));
                    writer.WriteLine(CenterText($"{"Less Withdrawal:",-20}{selectedSales.TotalPaymentsReceived,25:C}"));
                    writer.WriteLine(CenterText($"{"Payments Received:",-20}{selectedSales.TotalPaymentsReceived,25:C}"));
                    writer.WriteLine();
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine(CenterText($"{"Short/Over:",-20}{selectedSales.TotalShortOver,25:C}"));
                    writer.WriteLine(new string('-', receiptWidth));
                }

                if (File.Exists(filePath))
                {
                    await MessageBoxManager.GetMessageBoxStandard("Notification", "Printing receipt...", ButtonEnum.Ok).ShowAsPopupAsync(this);
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
            }
            catch (UnauthorizedAccessException)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", "Access Denied! Run as Administrator.", ButtonEnum.Ok).ShowAsPopupAsync(this);
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, ButtonEnum.Ok).ShowAsPopupAsync(this);
            }
        }







    }
};