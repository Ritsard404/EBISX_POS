using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Diagnostics;
using System.IO;

namespace EBISX_POS.Views
{
    public partial class CustomerInvoiceReceipt : Window
    {
        public CustomerInvoiceReceipt()
        {
            InitializeComponent();
            DataContext = new CustomerInvoiceReceiptViewModel();
        }

        private async void Button_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Set the target folder path – adjust to suit your environment.
                string folderPath = @"C:\Users\User\source\repos";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Define the full TXT file path.
                string fileName = "CustomerInvoiceReceipt.txt";
                string filePath = Path.Combine(folderPath, fileName);

                // Retrieve the view model.
                var viewModel = (CustomerInvoiceReceiptViewModel)DataContext;

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    int receiptWidth = 50; // Adjust for wider/narrower formatting

                    // Function to center text
                    string CenterText(string text) => text.PadLeft((receiptWidth + text.Length) / 2).PadRight(receiptWidth);

                    // Header
                    writer.WriteLine(new string('=', receiptWidth));
                    writer.WriteLine(CenterText("Customer Invoice Receipt"));
                    writer.WriteLine(new string('=', receiptWidth));
                    writer.WriteLine(CenterText(viewModel.BusinessName));
                    writer.WriteLine(CenterText(viewModel.Address));
                    writer.WriteLine(CenterText($"TIN: {viewModel.Tin}"));
                    writer.WriteLine(CenterText($"MIN: {viewModel.Min}"));
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();

                    // Invoice details
                    writer.WriteLine($"Invoice No: {viewModel.InvoiceNumber}".PadRight(receiptWidth - 10));
                    writer.WriteLine(CenterText(viewModel.InvoiceType));
                    writer.WriteLine($"Date: {viewModel.InvoiceDate:d}".PadRight(receiptWidth - 10));
                    writer.WriteLine($"Cashier: {viewModel.Cashier}".PadRight(receiptWidth - 10));
                    writer.WriteLine(new string('-', receiptWidth));

                    // Items header
                    writer.WriteLine($"{"Qty",-5} {"Description",-20} {"Price",-10} {"Amount",-10}");
                    writer.WriteLine(new string('-', receiptWidth));
                    writer.WriteLine();

                    // Invoice items
                    foreach (var item in viewModel.InvoiceItems)
                    {
                        writer.WriteLine($"{item.Quantity,-5} {item.Description,-20} {item.Price,-10:C} {item.Amount,-10:C}");
                    }
                    writer.WriteLine(new string('-', receiptWidth));

                    // Totals
                    writer.WriteLine(CenterText($"{"Total Amount:",-20}{viewModel.TotalAmount,20:C}"));
                    writer.WriteLine(CenterText($"{"Cash Received:",-20}{viewModel.CashReceived,20:C}"));
                    writer.WriteLine(CenterText($"{"Change:",-20}{viewModel.Change,20:C}"));
                    writer.WriteLine();
                    writer.WriteLine(CenterText($"{"Vatables Sales:",-20}{viewModel.VatableSales,20:C}"));
                    writer.WriteLine(CenterText($"{"Vat Exempt Sales:",-20}{viewModel.VatExemptSales,20:C}"));
                    writer.WriteLine(CenterText($"{"Vat Zero Sales:",-20}{viewModel.VatZeroRatedSales,20:C}"));
                    writer.WriteLine(CenterText($"{"Total VAT:",-20}{viewModel.TotalVat,20:C}"));
                    writer.WriteLine();


                    writer.WriteLine(CenterText($"Name:____________________________"));
                    writer.WriteLine(CenterText($"Address:_________________________"));
                    writer.WriteLine(CenterText("TIN: _____________________________"));
                    writer.WriteLine(CenterText("Signature: _______________________"));
                    writer.WriteLine();
                    
                  

                    // Footer
                    writer.WriteLine(CenterText("This Serve as Sales Invoice"));
                    writer.WriteLine(CenterText("Arsene Software Solutions"));
                    writer.WriteLine(CenterText("Labangon St. Cebu City, Cebu"));
                    writer.WriteLine(CenterText($"VAT Reg TIN: {viewModel.Tin}"));
                    writer.WriteLine(CenterText($"Date Issue: {viewModel.InvoiceDate:d}"));
                    writer.WriteLine(CenterText($"Valid Until: {viewModel.InvoiceDate:d}"));
                    writer.WriteLine();

                    writer.WriteLine(new string('=', receiptWidth));
                    writer.WriteLine(CenterText("Thank you for your purchase!"));
                    writer.WriteLine(new string('=', receiptWidth));

                    writer.WriteLine();
                    writer.WriteLine();

                }


                // Check if the file was successfully created
                if (File.Exists(filePath))
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Notification", "Printing Receipt...", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                }

                // Open the TXT file automatically.
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
}
