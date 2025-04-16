﻿using EBISX_POS.API.Services.DTO.Order;
using EBISX_POS.API.Services.DTO.Report;
using EBISX_POS.Services;
using EBISX_POS.Services.DTO.Report;
using EBISX_POS.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

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

        public static void PrintInvoice(string folderPath,string filePath, FinalizeOrderResponseDTO finalizeOrder)
        {
            // Ensure the target directory exists.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            using var writer = new StreamWriter(filePath);

            // Header
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine(CenterText("Customer Invoice Receipt"));
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine(CenterText(finalizeOrder.RegisteredName));
            writer.WriteLine(CenterText(finalizeOrder.Address));
            writer.WriteLine(CenterText($"TIN: {finalizeOrder.VatTinNumber}"));
            writer.WriteLine(CenterText($"MIN: {finalizeOrder.MinNumber}"));
            writer.WriteLine(new string('-', ReceiptWidth));
            writer.WriteLine();

            // Invoice details
            writer.WriteLine($"Invoice No: {finalizeOrder.InvoiceNumber}".PadRight(ReceiptWidth - 10));
            writer.WriteLine(CenterText(TenderState.tenderOrder.OrderType));
            writer.WriteLine($"Date: {finalizeOrder.InvoiceDate:d}".PadRight(ReceiptWidth - 10));
            writer.WriteLine($"Cashier: {CashierState.CashierName}".PadRight(ReceiptWidth - 10));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Items header// Items header
            writer.WriteLine($"{"Qty",-5} {"Description",-30} {"Amount",10}");
            writer.WriteLine(new string('-', ReceiptWidth));
            writer.WriteLine();


            // Invoice items
            foreach (var order in OrderState.CurrentOrder)
            {
                foreach (var item in order.DisplaySubOrders)
                {
                    // Simulate the grid column widths.
                    // Column 0: Fixed width (5 characters). Only show quantity if opacity is 1.
                    string quantityColumn = item.Opacity < 1.0
                        ? new string(' ', 5)
                        : $"{item.Quantity,-5}";

                    // Column 1: DisplayName in a left-aligned, fixed-width field (30 characters).
                    string displayNameColumn = $"{item.DisplayName,-30}";

                    // Column 2: Price string, right-aligned with 10 characters.
                    string priceColumn = item.IsUpgradeMeal ? $"{item.ItemPriceString,10}" : string.Empty;

                    // Write out the formatted line simulating the grid.
                    writer.WriteLine($"{quantityColumn}{displayNameColumn}{priceColumn}");
                }

                writer.WriteLine();
            }
            writer.WriteLine(new string('-', ReceiptWidth));

            // Totals
            writer.WriteLine(CenterText($"{"Total Amount:",-20}{TenderState.tenderOrder.TotalAmount.ToString("C", PesoCulture),20}"));
            if (TenderState.tenderOrder.HasOrderDiscount)
            {
                writer.WriteLine(CenterText($"{"Discount Amount:",-20}{TenderState.tenderOrder.DiscountAmount.ToString("C", PesoCulture),20}"));
            }
            writer.WriteLine(CenterText($"{"Due Amount:",-20}{TenderState.tenderOrder.AmountDue.ToString("C", PesoCulture),20}"));

            if (TenderState.tenderOrder.HasOtherPayments && TenderState.tenderOrder.OtherPayments != null)
            {
                foreach (var payment in TenderState.tenderOrder.OtherPayments)
                {
                    writer.WriteLine(CenterText($"{payment.SaleTypeName + ":",-20}{payment.Amount.ToString("C", PesoCulture),20}"));
                }
            }
            writer.WriteLine(CenterText($"{"Cash Tendered:",-20}{TenderState.tenderOrder.CashTenderAmount.ToString("C", PesoCulture),20}"));
            writer.WriteLine(CenterText($"{"Total Tendered:",-20}{TenderState.tenderOrder.TenderAmount.ToString("C", PesoCulture),20}"));
            writer.WriteLine(CenterText($"{"Change:",-20}{TenderState.tenderOrder.ChangeAmount.ToString("C", PesoCulture),20}"));
            writer.WriteLine();

            writer.WriteLine(CenterText($"{"Vat Zero Sales:",-20}{0.ToString("C", PesoCulture),20}"));
            writer.WriteLine(CenterText($"{"Vat Exempt Sales:",-20}{(TenderState.tenderOrder.VatExemptSales).ToString("C", PesoCulture),20}"));
            writer.WriteLine(CenterText($"{"Vatables Sales:",-20}{(TenderState.tenderOrder.VatSales).ToString("C", PesoCulture),20}"));
            writer.WriteLine(CenterText($"{"VAT Amount:",-20}{(TenderState.tenderOrder.VatAmount).ToString("C", PesoCulture),20}"));
            writer.WriteLine();

            if (TenderState.ElligiblePWDSCDiscount == null || !TenderState.ElligiblePWDSCDiscount.Any())
            {
                // Print the signature section once if no eligible discount state is available.
                writer.WriteLine(CenterText("Name:____________________________"));
                writer.WriteLine(CenterText("Address:_________________________"));
                writer.WriteLine(CenterText("TIN: _____________________________"));
                writer.WriteLine(CenterText("Signature: _______________________"));
                writer.WriteLine();
            }
            else
            {
                var names = (TenderState.ElligiblePWDSCDiscount?.Any() == true)
                    ? TenderState.ElligiblePWDSCDiscount
                    : new List<string> { string.Empty };

                foreach (var pwdSc in names)
                {
                    // Build the centered name text.
                    string nameText = $"Name: {pwdSc.ToUpper()}________";
                    writer.WriteLine(nameText);
                    // Create an underline using dashes; using the trimmed length of the nameText ensures a matching underline.
                    writer.WriteLine("Address: ___________________________");
                    writer.WriteLine("TIN: _____________________________");
                    writer.WriteLine("Signature: _______________________");
                    writer.WriteLine();
                }
            }


            // Footer
            writer.WriteLine(CenterText("This Serve as Sales Invoice"));
            writer.WriteLine(CenterText("Arsene Software Solutions"));
            writer.WriteLine(CenterText("Labangon St. Cebu City, Cebu"));
            writer.WriteLine(CenterText($"VAT Reg TIN: {finalizeOrder.VatTinNumber}"));
            writer.WriteLine(CenterText($"Date Issue: {finalizeOrder.DateIssued:d}"));
            writer.WriteLine(CenterText($"Valid Until: {finalizeOrder.ValidUntil:d}"));
            writer.WriteLine();
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine(CenterText("Thank you for your purchase!"));
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine();
        }

        public static void PrintSearchedInvoice(string folderPath, string filePath, InvoiceDetailsDTO invoice)
        {
            // Ensure the target directory exists.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using var writer = new StreamWriter(filePath);

            // Header
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine(CenterText("Customer Invoice Receipt"));
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine(CenterText(invoice.RegisteredName));
            writer.WriteLine(CenterText(invoice.Address));
            writer.WriteLine(CenterText($"TIN: {invoice.VatTinNumber}"));
            writer.WriteLine(CenterText($"MIN: {invoice.MinNumber}"));
            writer.WriteLine(new string('-', ReceiptWidth));
            writer.WriteLine();

            // Invoice details
            writer.WriteLine($"Invoice No: {invoice.InvoiceNum}".PadRight(ReceiptWidth - 10));
            writer.WriteLine(CenterText(invoice.OrderType));
            writer.WriteLine($"Date: {invoice.InvoiceDate:d}".PadRight(ReceiptWidth - 10));
            writer.WriteLine($"Cashier: {invoice.CashierName}".PadRight(ReceiptWidth - 10));
            writer.WriteLine(new string('-', ReceiptWidth));

            // Items header
            writer.WriteLine($"{"Qty",-5} {"Description",-30} {"Amount",10}");
            writer.WriteLine(new string('-', ReceiptWidth));
            writer.WriteLine();

            // Invoice items
            foreach (var item in invoice.Items)
            {
                foreach (var itemInfo in item.itemInfos)
                {
                    // Column 0: Fixed width (5 characters). Only show quantity if opacity is 1.
                    string quantityColumn = $"{item.Qty,-5}";

                    // Column 1: Description in a left-aligned, fixed-width field (30 characters).
                    string descriptionColumn = $"{itemInfo.Description,-30}";

                    // Column 2: Amount, right-aligned with 10 characters.
                    string amountColumn = $"{itemInfo.Amount,10}";

                    // Write out the formatted line simulating the grid.
                    writer.WriteLine($"{quantityColumn}{descriptionColumn}{amountColumn}");
                }
            }
            writer.WriteLine(new string('-', ReceiptWidth));

            // Totals
            writer.WriteLine(CenterText($"{"Total Amount:",-20}{invoice.TotalAmount,20}"));
            if (!string.IsNullOrEmpty(invoice.DiscountAmount))
            {
                writer.WriteLine(CenterText($"{"Discount Amount:",-20}{invoice.DiscountAmount,20}"));
            }
            writer.WriteLine(CenterText($"{"Due Amount:",-20}{invoice.DueAmount,20}"));

            if (invoice.OtherPayments != null && invoice.OtherPayments.Any())
            {
                foreach (var payment in invoice.OtherPayments)
                {
                    writer.WriteLine(CenterText($"{payment.SaleTypeName + ":",-20}{payment.Amount,20}"));
                }
            }
            writer.WriteLine(CenterText($"{"Cash Tendered:",-20}{invoice.CashTenderAmount,20}"));
            writer.WriteLine(CenterText($"{"Total Tendered:",-20}{invoice.TotalTenderAmount,20}"));
            writer.WriteLine(CenterText($"{"Change:",-20}{invoice.ChangeAmount,20}"));
            writer.WriteLine();

            writer.WriteLine(CenterText($"{"Vat Zero Sales:",-20}{0.ToString("C", PesoCulture),20}"));
            writer.WriteLine(CenterText($"{"Vat Exempt Sales:",-20}{invoice.VatExemptSales,20}"));
            writer.WriteLine(CenterText($"{"Vatables Sales:",-20}{invoice.VatSales,20}"));
            writer.WriteLine(CenterText($"{"VAT Amount:",-20}{invoice.VatAmount,20}"));
            writer.WriteLine();

            if (invoice.ElligiblePeopleDiscounts == null || !invoice.ElligiblePeopleDiscounts.Any())
            {
                // Print the signature section once if no eligible discount state is available.
                writer.WriteLine(CenterText("Name:____________________________"));
                writer.WriteLine(CenterText("Address:_________________________"));
                writer.WriteLine(CenterText("TIN: _____________________________"));
                writer.WriteLine(CenterText("Signature: _______________________"));
                writer.WriteLine();
            }
            else
            {
                foreach (var pwdSc in invoice.ElligiblePeopleDiscounts)
                {
                    // Build the centered name text.
                    string nameText = $"Name: {pwdSc.ToUpper()}________";
                    writer.WriteLine(nameText);
                    writer.WriteLine("Address: ___________________________");
                    writer.WriteLine("TIN: _____________________________");
                    writer.WriteLine("Signature: _______________________");
                    writer.WriteLine();
                }
            }

            // Footer
            writer.WriteLine(CenterText("This Serve as Sales Invoice"));
            writer.WriteLine(CenterText("Arsene Software Solutions"));
            writer.WriteLine(CenterText("Labangon St. Cebu City, Cebu"));
            writer.WriteLine(CenterText($"VAT Reg TIN: {invoice.VatTinNumber}"));
            writer.WriteLine(CenterText($"Date Issue: {invoice.DateIssued:d}"));
            writer.WriteLine(CenterText($"Valid Until: {invoice.ValidUntil:d}"));
            writer.WriteLine();
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine(CenterText("Thank you for your purchase!"));
            writer.WriteLine(new string('=', ReceiptWidth));
            writer.WriteLine();

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

    }
}
