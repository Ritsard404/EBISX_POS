using EBISX_POS.API.Services.DTO.Report;
using EBISX_POS.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace EBISX_POS.Util
{
    public static class ReceiptPrinterUtil
    {
        private const int ReceiptWidth = 50;
        private static readonly CultureInfo PesoCulture = new CultureInfo("en-PH");

        private static string CenterText(string text) =>
            text.PadLeft((ReceiptWidth + text.Length) / 2).PadRight(ReceiptWidth);

        private static string AlignText(string left, string right) =>
            left.PadRight(ReceiptWidth - right.Length) + right;

        public static async void PrintXReading(IServiceProvider serviceProvider)
        {
            var reportOptions = serviceProvider.GetRequiredService<IOptions<SalesReport>>();
            var reportService = serviceProvider.GetRequiredService<ReportService>();

            var rpt = await reportService.XInvoiceReport();

            string fileName = $"XInvoice-{DateTimeOffset.UtcNow.ToString("MMMM-dd-yyyy-HH-mm-ss")}.txt";
            var filePath = Path.Combine(reportOptions.Value.XInvoiceReport, fileName);

            if (!Directory.Exists(reportOptions.Value.XInvoiceReport))
            {
                Directory.CreateDirectory(reportOptions.Value.XInvoiceReport);
            }

            using var writer = new StreamWriter(filePath);

            // Header
            writer.WriteLine(CenterText(rpt.BusinessName));
            writer.WriteLine(CenterText($"Operated by: {rpt.OperatorName}"));
            writer.WriteLine();
            writer.WriteLine(CenterText(rpt.AddressLine));
            writer.WriteLine();
            writer.WriteLine(CenterText($"VAT REG TIN: {rpt.VatRegTin}"));
            writer.WriteLine(CenterText($"MIN: {rpt.Min}"));
            writer.WriteLine(CenterText($"S/N: {rpt.SerialNumber}"));
            writer.WriteLine();

            // Title
            writer.WriteLine(CenterText("X-READING REPORT"));
            writer.WriteLine();

            // Report date/time
            writer.WriteLine(AlignText("Report Date:", rpt.ReportDate));
            writer.WriteLine(AlignText("Report Time:", rpt.ReportTime));
            writer.WriteLine();

            // Period
            writer.WriteLine(AlignText("Start Date & Time:", rpt.StartDateTime));
            writer.WriteLine(AlignText("End Date & Time:", rpt.EndDateTime));
            writer.WriteLine();

            // Cashier & OR
            writer.WriteLine(AlignText("Cashier:", rpt.Cashier));
            writer.WriteLine();
            writer.WriteLine(AlignText("Beg. OR #:", rpt.BeginningOrNumber));
            writer.WriteLine(AlignText("End. OR #:", rpt.EndingOrNumber));
            writer.WriteLine();

            // Opening fund
            writer.WriteLine(AlignText("Opening Fund:", rpt.OpeningFund));
            writer.WriteLine(new string('=', ReceiptWidth));

            // Payments section
            writer.WriteLine(CenterText("PAYMENTS RECEIVED"));
            writer.WriteLine();
            writer.WriteLine(AlignText("CASH", rpt.Payments.CashString));
            foreach (var p in rpt.Payments.OtherPayments)
            {
                writer.WriteLine(AlignText(p.Name.ToUpper(), p.AmountString));
            }
            writer.WriteLine(AlignText("Total Payments:", rpt.Payments.Total));
            writer.WriteLine(new string('=', ReceiptWidth));

            // Void / Refund / Withdrawal
            writer.WriteLine(AlignText("VOID", rpt.VoidAmount));
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine(AlignText("REFUND", rpt.Refund));
            writer.WriteLine(new string('=', ReceiptWidth));

            // Transaction summary
            writer.WriteLine(CenterText("TRANSACTION SUMMARY"));
            writer.WriteLine();
            writer.WriteLine(AlignText("Cash In Drawer:", rpt.TransactionSummary.CashInDrawer));
            foreach (var p in rpt.TransactionSummary.OtherPayments)
            {
                writer.WriteLine(AlignText(p.Name.ToUpper(), p.AmountString));
            }
            writer.WriteLine(new string('=', ReceiptWidth));

            // Short/Over
            writer.WriteLine(AlignText("SHORT/OVER:", rpt.ShortOver));
            writer.WriteLine();

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
