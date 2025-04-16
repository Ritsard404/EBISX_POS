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
            writer.WriteLine(AlignText("WITHDRAWAL", rpt.Withdrawal));
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

        public static async void PrintZReading(IServiceProvider serviceProvider)
        {
            var reportOptions = serviceProvider.GetRequiredService<IOptions<SalesReport>>();
            var reportService = serviceProvider.GetRequiredService<ReportService>();

            var rpt = await reportService.ZInvoiceReport(); // You should implement this accordingly

            string fileName = $"ZInvoice-{DateTimeOffset.UtcNow:MMMM-dd-yyyy-HH-mm-ss}.txt";
            var filePath = Path.Combine(reportOptions.Value.ZInvoiceReport, fileName);

            if (!Directory.Exists(reportOptions.Value.ZInvoiceReport))
            {
                Directory.CreateDirectory(reportOptions.Value.ZInvoiceReport);
            }

            using var writer = new StreamWriter(filePath);

            // Header
            writer.WriteLine(CenterText(rpt.BusinessName));
            writer.WriteLine(CenterText($"Operated by: {rpt.OperatorName}"));
            writer.WriteLine(CenterText(rpt.AddressLine));
            writer.WriteLine(CenterText($"VAT REG TIN: {rpt.VatRegTin}"));
            writer.WriteLine(CenterText($"MIN: {rpt.Min}"));
            writer.WriteLine(CenterText($"S/N: {rpt.SerialNumber}"));
            writer.WriteLine();

            // Title
            writer.WriteLine(CenterText("Z-READING REPORT"));
            writer.WriteLine();

            // Report date/time
            writer.WriteLine(AlignText("Report Date:", rpt.ReportDate));
            writer.WriteLine(AlignText("Report Time:", rpt.ReportTime));
            writer.WriteLine();

            // Period
            writer.WriteLine(AlignText("Start Date & Time:", rpt.StartDateTime));
            writer.WriteLine(AlignText("End Date & Time:", rpt.EndDateTime));
            writer.WriteLine();

            // SI/VOID/RETURN numbers
            writer.WriteLine(AlignText("Beg. SI #:", rpt.BeginningSI));
            writer.WriteLine(AlignText("End. SI #:", rpt.EndingSI));
            writer.WriteLine(AlignText("Beg. VOID #:", rpt.BeginningVoid));
            writer.WriteLine(AlignText("End. VOID #:", rpt.EndingVoid));
            writer.WriteLine(AlignText("Beg. RETURN #:", rpt.BeginningReturn));
            writer.WriteLine(AlignText("End. RETURN #:", rpt.EndingReturn));
            writer.WriteLine();

            writer.WriteLine(AlignText("Reset Counter No.:", rpt.ResetCounter));
            writer.WriteLine(AlignText("Z Counter No.:", rpt.ZCounter));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Sales section
            writer.WriteLine(AlignText("Present Accumulated Sales:", rpt.PresentAccumulatedSales));
            writer.WriteLine(AlignText("Previous Accumulated Sales:", rpt.PreviousAccumulatedSales));
            writer.WriteLine(AlignText("Sales for the Day:", rpt.SalesForTheDay));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Breakdown of sales
            writer.WriteLine(CenterText("BREAKDOWN OF SALES"));
            writer.WriteLine();
            writer.WriteLine(AlignText("VATABLE SALES:", rpt.SalesBreakdown.VatableSales));
            writer.WriteLine(AlignText("VAT AMOUNT:", rpt.SalesBreakdown.VatAmount));
            writer.WriteLine(AlignText("VAT EXEMPT SALES:", rpt.SalesBreakdown.VatExemptSales));
            writer.WriteLine(AlignText("ZERO RATED SALES:", rpt.SalesBreakdown.ZeroRatedSales));
            writer.WriteLine(new string('-', ReceiptWidth));
            writer.WriteLine(AlignText("Gross Amount:", rpt.SalesBreakdown.GrossAmount));
            writer.WriteLine(AlignText("Less Discount:", rpt.SalesBreakdown.LessDiscount));
            writer.WriteLine(AlignText("Less Return:", rpt.SalesBreakdown.LessReturn));
            writer.WriteLine(AlignText("Less Void:", rpt.SalesBreakdown.LessVoid));
            writer.WriteLine(AlignText("Less VAT Adjustment:", rpt.SalesBreakdown.LessVatAdjustment));
            writer.WriteLine(AlignText("Net Amount:", rpt.SalesBreakdown.NetAmount));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Discounts
            writer.WriteLine(CenterText("DISCOUNT SUMMARY"));
            writer.WriteLine(AlignText("SC Disc. :", rpt.DiscountSummary.SeniorCitizen));
            writer.WriteLine(AlignText("PWD Disc. :", rpt.DiscountSummary.Pwd));
            writer.WriteLine(AlignText("Other Disc. :", rpt.DiscountSummary.Other));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Adjustments
            writer.WriteLine(CenterText("SALES ADJUSTMENT"));
            writer.WriteLine(AlignText("VOID :", rpt.SalesAdjustment.Void));
            writer.WriteLine(AlignText("RETURN :", rpt.SalesAdjustment.Return));
            writer.WriteLine(new string('-', ReceiptWidth));

            writer.WriteLine(CenterText("VAT ADJUSTMENT"));
            writer.WriteLine(AlignText("SC TRANS. :", rpt.VatAdjustment.ScTrans));
            writer.WriteLine(AlignText("PWD TRANS :", rpt.VatAdjustment.PwdTrans));
            writer.WriteLine(AlignText("REG.Disc. TRANS :", rpt.VatAdjustment.RegDiscTrans));
            writer.WriteLine(AlignText("ZERO-RATED TRANS.:", rpt.VatAdjustment.ZeroRatedTrans));
            writer.WriteLine(AlignText("VAT on Return:", rpt.VatAdjustment.VatOnReturn));
            writer.WriteLine(AlignText("Other VAT Adjustments:", rpt.VatAdjustment.OtherAdjustments));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Transaction Summary
            writer.WriteLine(CenterText("TRANSACTION SUMMARY"));
            writer.WriteLine();
            writer.WriteLine(AlignText("Cash In Drawer:", rpt.TransactionSummary.CashInDrawer));
            foreach (var p in rpt.TransactionSummary.OtherPayments)
            {
                writer.WriteLine(AlignText(p.Name.ToUpper(), p.AmountString));
            }
            writer.WriteLine(AlignText("Opening Fund:", rpt.OpeningFund));
            writer.WriteLine(AlignText("Less Withdrawal:", rpt.Withdrawal));
            writer.WriteLine(AlignText("Payments Received:", rpt.PaymentsReceived));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Short/Over
            writer.WriteLine(AlignText("SHORT/OVER:", rpt.ShortOver));
            writer.WriteLine();

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

    }
}
