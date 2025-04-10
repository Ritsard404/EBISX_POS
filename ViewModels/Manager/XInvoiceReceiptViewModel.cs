using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace EBISX_POS.ViewModels.Manager
{
    public class SalesReportViewModel : ObservableObject
    {
        public SalesReport Report { get; set; }

        public SalesReportViewModel()
        {
            Report = new SalesReport
            {
                Name = "Jollibee Gen. Maxilom",
                Address = "1234 Jollibee St., Cebu City",
                TIN = "123-456-789-000",
                MIN = "123456789",
                StoreCode = "JB3095",
                InvoiceType = "Sales Invoice",
                SNumber = "1234567890",
                CashierName = "Nashley Taba",
                StartReportDate = new DateTime(2021, 3, 15),
                EndReportDate = new TimeSpan(11, 30, 0),
                StartTime = new DateTime(2021, 3, 15, 9, 0, 0),
                EndTime = new DateTime(2021, 3, 15, 11, 29, 0),
                BegSI = "00000000000001",
                LastORNumber = "000006",
                BegVOID = "00000000000001",
                EndVOID = "00000000000001",
                BegRETURN = "00000000000000",
                EndRETURN = "00000000000000",
                Items = new ObservableCollection<InvoiceItem>
                {
                    new InvoiceItem { Quantity = 2, Description = "Chicken Joy (2pcs)", Price = 200.00m },
                    new InvoiceItem { Quantity = 1, Description = "Jolly Spaghetti", Price = 50.00m },
                    new InvoiceItem { Quantity = 1, Description = "Burger Steak (1pc)", Price = 60.00m },
                    new InvoiceItem { Quantity = 1, Description = "Jolly Hotdog Classic", Price = 75.00m },
                    new InvoiceItem { Quantity = 1, Description = "Fries (Large)", Price = 45.00m },
                    new InvoiceItem { Quantity = 2, Description = "Yumburger", Price = 80.00m },
                    new InvoiceItem { Quantity = 1, Description = "Jolly Crispy Fries Bucket", Price = 150.00m },
                    new InvoiceItem { Quantity = 1, Description = "Jolly Kiddie Meal (Burger & Fries)", Price = 120.00m },
                    new InvoiceItem { Quantity = 1, Description = "Peach Mango Pie", Price = 35.00m },
                    new InvoiceItem { Quantity = 1, Description = "Jolly Sundae (Chocolate)", Price = 30.00m },
                    new InvoiceItem { Quantity = 1, Description = "Coffee Float", Price = 55.00m },
                    new InvoiceItem { Quantity = 1, Description = "Jolly Hotdog Cheesy Classic", Price = 90.00m },
                    new InvoiceItem { Quantity = 2, Description = "Rice", Price = 50.00m },
                    new InvoiceItem { Quantity = 1, Description = "Choco Mallow Pie", Price = 40.00m },
                    new InvoiceItem { Quantity = 1, Description = "Coke (Large)", Price = 55.00m }
                },
                ResetCounterNo = 0,
                ZCounterNo = 1,

                PresentAccumulatedSales = 1206.00m,
                PreviousAccumulatedSales = 0.00m,
                SalesForTheDay = 1206.00m,

                VATableSales = 533.74m,
                VATAmount = 64.06m,
                VATExemptSales = 359.43m,
                ZeroRatedSales = 0.00m,

                GrossAmount = 1206.00m,
                Discount = 51.83m,
                Return = 0.00m,
                Void = 214.00m,
                VATAdjustment = 6.02m,
                NetSalesAmount = 1148.14m,

                SCDiscount = 467.35m,
                PWDDiscount = 0.00m,
                NAACDiscount = 0.00m,
                SoloParentDiscount = 0.00m,
                TotalDiscount = 0.00m,

                VoidAdjustment = 214.00m,
                ReturnAdjustment = 0.00m,

                SCTrans = 0.38m,
                PWDTrans = 2.96m,
                RegDiscTrans = 2.68m,
                ZeroRatedTrans = 0.00m,
                VATOnReturn = 0.00m,
                OtherVATAdjustments = 0.00m,

                CashInDrawer = 635.00m,
                Cheque = 100.00m,
                CreditCard = 100.00m,
                GiftCertificate = 100.00m,
                OpeningFund = 0.00m,
                Withdrawal = 0.00m,
                PaymentsReceived = 934.14m,
                ShortOver = 0.86m,
             
            };
        }
    }

    public class SalesReport
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
        public string LastORNumber { get; set; }
        public string BegVOID { get; set; }
        public string EndVOID { get; set; }
        public string BegRETURN { get; set; }
        public string EndRETURN { get; set; }
        public ObservableCollection<InvoiceItem> Items { get; set; } = new();
        public int ResetCounterNo { get; set; }
        public int ZCounterNo { get; set; }

        public decimal PresentAccumulatedSales { get; set; }
        public decimal PreviousAccumulatedSales { get; set; }
        public decimal SalesForTheDay { get; set; }

        public decimal VATableSales { get; set; }
        public decimal VATAmount { get; set; }
        public decimal VATExemptSales { get; set; }
        public decimal ZeroRatedSales { get; set; }

        public decimal GrossAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Return { get; set; }
        public decimal Void { get; set; }
        public decimal VATAdjustment { get; set; }
        public decimal NetSalesAmount { get; set; }

        public decimal SCDiscount { get; set; }
        public decimal PWDDiscount { get; set; }
        public decimal NAACDiscount { get; set; }
        public decimal SoloParentDiscount { get; set; }
        public decimal TotalDiscount { get; set; }

        public decimal VoidAdjustment { get; set; }
        public decimal ReturnAdjustment { get; set; }

        public decimal SCTrans { get; set; }
        public decimal PWDTrans { get; set; }
        public decimal RegDiscTrans { get; set; }
        public decimal ZeroRatedTrans { get; set; }
        public decimal VATOnReturn { get; set; }
        public decimal OtherVATAdjustments { get; set; }

        public decimal CashInDrawer { get; set; }
        public decimal Cheque { get; set; }
        public decimal CreditCard { get; set; }
        public decimal GiftCertificate { get; set; }
        public decimal OpeningFund { get; set; }
        public decimal Withdrawal { get; set; }
        public decimal PaymentsReceived { get; set; }

        public decimal ShortOver { get; set; }
    }

    public class InvoiceItem
    {
        private string _description = string.Empty;
        private const int MaxDescriptionLength = 12;

        public int Quantity { get; set; }

        public string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _description = string.Empty;
                else
                    _description = value.Length > MaxDescriptionLength ? value.Substring(0, MaxDescriptionLength) : value;
            }
        }

        public decimal Price { get; set; }
    }
}
