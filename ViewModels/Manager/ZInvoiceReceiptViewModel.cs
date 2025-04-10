using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace EBISX_POS.ViewModels.Manager
{
    class ZInvoiceReceiptViewModel : ObservableObject
    {
        public ZSalesReport Report { get; set; }

        public ZInvoiceReceiptViewModel()
        {
            Report = new ZSalesReport
            {
                Name = "Jollibee Gen. Maxilom",
                Address = "1234 Jollibee St., Cebu City",
                TIN = "123-456-789-000",
                MIN = "123456789",
                StoreCode = "JB3095",
                InvoiceType = "Sales Invoice",
                CashierName = "Nashley Taba",
                SNumber = "12312323-23202",
                StartReportDate = new DateTime(2024, 4, 1),
                EndReportDate = new TimeSpan(23, 59, 59), // Represents end of day
                StartTime = new DateTime(2024, 4, 1, 8, 0, 0),
                EndTime = new DateTime(2024, 4, 1, 23, 59, 0),
                BegSI = "00000000000001",
                EndSI = "000006",
                BegVOID = "00000000000001",
                EndVOID = "00000000000002",
                BegRETURN = "00000000000000",
                EndRETURN = "00000000000001",
                TotalSales = 5000.00m, // Gross Sales
                TotalReturn = 200.00m, // Total Return
                TotalReturnAdjustment = 50.00m, // Adjustments on returns
                TotalDiscount = 300.00m, // Total Discounts Given
                TotalNetSales = 5000.00m - 200.00m - 300.00m, // Gross Sales - Returns - Discounts
                TotalVATableSales = (5000.00m - 200.00m - 300.00m) * 0.88m, // Assuming 88% of net sales are vatable
                TotalVATAmount = (5000.00m - 200.00m - 300.00m) * 0.12m, // 12% VAT
                TotalVATExemptSales = 100.00m, // Sales not subject to VAT
                TotalZeroRatedSales = 50.00m, // Zero-rated sales
                TotalVoid = 150.00m, // Total value of voided transactions
                TotalVoidAdjustment = 30.00m, // Adjustments on voids
                CashInDrawer = 2000.00m, // Cash present in drawer
                CreditCard = 1500.00m, // Credit card transactions
                Cheque = 500.00m, // Cheque payments
                GiftCertificate = 300.00m, // Gift certificate payments
                TotalPaymentsReceived = 2000.00m + 1500.00m + 500.00m + 300.00m, // Sum of all payment methods
                TotalShortOver = 20.00m, // Cash discrepancy
                TotalNumVoid = 5, // Number of void transactions
                DineIn = 3500.00m, // Sales from Dine-in customers
                TakeOut = 1500.00m, // Sales from Take-out customers
                PaxCount = 75, // Total number of customers served
                LastORNumber = 000093, // Last OR number issued
                AvgTrasaction = 5000.00m / 75, // Average per transaction = Gross Sales / Pax Count
                PresentAccumulatedSales = 10000.00m, // Sales carried over
                PreviousAccumulatedSales = 5000.00m, // Past recorded sales
                SalesForTheDay = 5000.00m + 5000.00m, // Total accumulated sales
                SCDiscount = 100.00m, // Senior Citizen Discount
                PWDDiscount = 50.00m, // PWD Discount
                ZReset = 0, // Z Reset Counter

            };
        }
    }

    public class ZSalesReport
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string TIN { get; set; }
        public string MIN { get; set; }
        public string CashierName { get; set; }
        public string StoreCode { get; set; }
        public string InvoiceType { get; set; }
        public string SNumber { get; set; }
        public DateTime StartReportDate { get; set; }
        public TimeSpan EndReportDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string BegSI { get; set; }
        public string EndSI { get; set; }
        public string BegVOID { get; set; }
        public string EndVOID { get; set; }
        public string BegRETURN { get; set; }
        public string EndRETURN { get; set; }
        public int LastORNumber { get; set; }
        public int PaxCount { get; set; }
        public decimal AvgTrasaction { get; set; }
        public decimal DineIn { get; set; }
        public decimal TakeOut { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal CashInDrawer { get; set; }
        public decimal Cheque { get; set; }
        public decimal CreditCard { get; set; }
        public decimal GiftCertificate { get; set; }
        public decimal TotalSales { get; set; } // Gross Sales
        public decimal TotalVATableSales { get; set; } // 88% of Net Sales
        public decimal TotalVATAmount { get; set; } // 12% of Net Sales
        public decimal TotalVATExemptSales { get; set; } // Fixed value
        public decimal TotalZeroRatedSales { get; set; } // Fixed value
        public decimal TotalNetSales { get; set; } // Gross Sales - Return - Discounts
        public decimal TotalVoid { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal TotalVoidAdjustment { get; set; }
        public decimal TotalReturnAdjustment { get; set; }
        public decimal TotalCashInDrawer { get; set; }
        public decimal TotalCheque { get; set; }
        public decimal TotalCreditCard { get; set; }
        public decimal TotalGiftCertificate { get; set; }
        public decimal TotalPaymentsReceived { get; set; } // Sum of all payment methods
        public decimal TotalShortOver { get; set; } // Cash discrepancy
        public decimal TotalNumVoid { get; set; }
        public decimal ZReset { get; set; } // Z Reset Counter
        public decimal SCDiscount { get; set; } // Senior Citizen Discount
        public decimal PWDDiscount { get; set; } // PWD Discount
        public decimal PresentAccumulatedSales { get; set; } // Carried over sales
        public decimal PreviousAccumulatedSales { get; set; } // Past recorded sales
        public decimal SalesForTheDay { get; set; } // Sum of previous + current day sales
    }
}
