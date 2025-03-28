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
using System.Threading.Tasks;

namespace EBISX_POS.Views
{
    public partial class CustomerInvoiceReceipt : Window
    {
        public CustomerInvoiceReceipt()
        {
            InitializeComponent();
            DataContext = new CustomerInvoiceReceiptViewModel();
        }

        /// <summary>
        /// Generates a receipt file from the provided view model data and opens it.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task GenerateAndPrintReceiptAsync()
        {
            // Define target folder and file paths.
            string folderPath = @"C:\POS\Reciepts";
            string fileName = "CustomerInvoiceReceipt.txt";
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                // Ensure the target directory exists.
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Retrieve the view model from DataContext.
                if (!(DataContext is CustomerInvoiceReceiptViewModel viewModel))
                {
                    throw new InvalidOperationException("Invalid DataContext. Expected CustomerInvoiceReceiptViewModel.");
                }

                // Write receipt content to file.
                WriteReceiptContent(filePath, viewModel);

                // Notify the user if the file was created.
                if (File.Exists(filePath))
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Notification", "Printing Receipt...", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                }

                // Open the receipt file automatically.
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

        /// <summary>
        /// Writes the receipt content to the specified file path using data from the view model.
        /// </summary>
        /// <param name="filePath">The full file path where the receipt will be written.</param>
        /// <param name="viewModel">The view model containing receipt data.</param>
        private void WriteReceiptContent(string filePath, CustomerInvoiceReceiptViewModel viewModel)
        {
            int receiptWidth = 50; // Adjust as necessary for formatting

            // Local helper to center text.
            string CenterText(string text) =>
                text.PadLeft((receiptWidth + text.Length) / 2).PadRight(receiptWidth);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
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

                // Signature section
                writer.WriteLine(CenterText("Name:____________________________"));
                writer.WriteLine(CenterText("Address:_________________________"));
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
            }
        }

        /// <summary>
        /// Event handler for the receipt generation button click.
        /// </summary>
        private async void Button_Click(object? sender, RoutedEventArgs e)
        {
            // Kick off the asynchronous receipt generation task.
            await GenerateAndPrintReceiptAsync();
        }

    }
}
