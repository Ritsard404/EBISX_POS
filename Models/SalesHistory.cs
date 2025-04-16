using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Models
{
    public class SalesHistory
    {
        public string ReceiptNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TIN { get; set; }
        public string MIN { get; set; }
        public string CashierName { get; set; }
        public string StoreCode { get; set; }
        public string InvoiceType { get; set; }
        public string SNumber { get; set; }
        // Changed from DateTimeOffset to string
        public DateTimeOffset StartReportDate { get; set; }
        public DateTimeOffset EndReportDate { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
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

        // Computed property for formatted date (only the date part)
        public string StartReportDateFormatted => StartReportDate.ToString("MM/dd/yyyy");

        // Computed property for formatted time (only the time part)
        public string StartTimeFormatted => StartTime.ToString("hh:mm tt");

        // Optionally add similar properties for EndReportDate and EndTime if needed.
        public string EndReportDateFormatted => EndReportDate.ToString("MM/dd/yyyy");
        public string EndTimeFormatted => EndTime.ToString("hh:mm tt");



        public int CrewMemberId { get; set; } // Foreign key for CrewMember

        public SalesHistory(
            string receiptNumber,
            string product,
            int quantity,
            decimal price,
            decimal total,
            DateTime date,
            int crewMemberId,
            CrewMember crewMember,

            // Additional properties
            string name,
            string address,
            string tin,
            string min,
            string cashierName,
            string storeCode,
            string invoiceType,
            string sNumber,
            DateTimeOffset startReportDate,
            DateTimeOffset endReportDate,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            string begSI,
            string endSI,
            string begVOID,
            string endVOID,
            string begRETURN,
            string endRETURN,
            int lastORNumber,
            int paxCount,
            decimal avgTransaction,
            decimal dineIn,
            decimal takeOut,
            decimal totalDiscount,
            decimal cashInDrawer,
            decimal cheque,
            decimal creditCard,
            decimal giftCertificate,
            decimal totalSales,
            decimal totalVATableSales,
            decimal totalVATAmount,
            decimal totalVATExemptSales,
            decimal totalZeroRatedSales,
            decimal totalNetSales,
            decimal totalVoid,
            decimal totalReturn,
            decimal totalVoidAdjustment,
            decimal totalReturnAdjustment,
            decimal totalCashInDrawer,
            decimal totalCheque,
            decimal totalCreditCard,
            decimal totalGiftCertificate,
            decimal totalPaymentsReceived,
            decimal totalShortOver,
            decimal totalNumVoid,
            decimal zReset,
            decimal scDiscount,
            decimal pwdDiscount,
            decimal presentAccumulatedSales,
            decimal previousAccumulatedSales,
            decimal salesForTheDay
)
        {
            ReceiptNumber = receiptNumber;
            CrewMemberId = crewMemberId;

            Name = name;
            Address = address;
            TIN = tin;
            MIN = min;
            CashierName = cashierName;
            StoreCode = storeCode;
            InvoiceType = invoiceType;
            SNumber = sNumber;
            StartReportDate = startReportDate;
            EndReportDate = endReportDate;
            StartTime = startTime;
            EndTime = endTime;
            BegSI = begSI;
            EndSI = endSI;
            BegVOID = begVOID;
            EndVOID = endVOID;
            BegRETURN = begRETURN;
            EndRETURN = endRETURN;
            LastORNumber = lastORNumber;
            PaxCount = paxCount;
            AvgTrasaction = avgTransaction;
            DineIn = dineIn;
            TakeOut = takeOut;
            TotalDiscount = totalDiscount;
            CashInDrawer = cashInDrawer;
            Cheque = cheque;
            CreditCard = creditCard;
            GiftCertificate = giftCertificate;
            TotalSales = totalSales;
            TotalVATableSales = totalVATableSales;
            TotalVATAmount = totalVATAmount;
            TotalVATExemptSales = totalVATExemptSales;
            TotalZeroRatedSales = totalZeroRatedSales;
            TotalNetSales = totalNetSales;
            TotalVoid = totalVoid;
            TotalReturn = totalReturn;
            TotalVoidAdjustment = totalVoidAdjustment;
            TotalReturnAdjustment = totalReturnAdjustment;
            TotalCashInDrawer = totalCashInDrawer;
            TotalCheque = totalCheque;
            TotalCreditCard = totalCreditCard;
            TotalGiftCertificate = totalGiftCertificate;
            TotalPaymentsReceived = totalPaymentsReceived;
            TotalShortOver = totalShortOver;
            TotalNumVoid = totalNumVoid;
            ZReset = zReset;
            SCDiscount = scDiscount;
            PWDDiscount = pwdDiscount;
            PresentAccumulatedSales = presentAccumulatedSales;
            PreviousAccumulatedSales = previousAccumulatedSales;
            SalesForTheDay = salesForTheDay;
        }

        public SalesHistory() { }

        // ✅ Tip: If this gets too long, you can use an object initializer or a builder pattern, especially for reporting-type models like this one.
    }
}
