using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels.Manager
{
    public partial class SalesHistoryViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SalesHistory> salesHistoryList = new();

        [ObservableProperty]
        private SalesHistory selectedRecord;

        [ObservableProperty]
        private DateTimeOffset selectedDate = new DateTimeOffset(new DateTime(2010, 1, 1));

        private readonly List<SalesHistory> allSalesHistory; //

        public IAsyncRelayCommand<SalesHistory> PrintReceiptCommand { get; }

        [ObservableProperty]
        private string searchReceiptNumber;

        public SalesHistoryViewModel()
        {


            // Initialize sample data
            allSalesHistory = new List<SalesHistory>
            {
                new SalesHistory
                {
                    ReceiptNumber = "R001",
                    CrewMemberId = 1 ,// Only set the CrewMemberId

                    Name = "Jollibee Gen. Maxilom",
                    Address = "1234 Jollibee St., Cebu City",
                    TIN = "123-456-789-000",
                    MIN = "123456789",
                    CashierName = "Nashley Taba",
                    StoreCode = "JB-01",
                    InvoiceType = "Sales Invoice",
                    SNumber = "1234567890-01",
                    StartReportDate = DateTimeOffset.Now.Date.AddDays(-2),
                    EndReportDate = DateTimeOffset.Now.Date.AddDays(-1),
                    StartTime = DateTimeOffset.Now.Date.AddDays(-1),
                    EndTime = DateTimeOffset.Now.Date.AddDays(-1),

                    BegSI = "JB3090",
                    EndSI = "JB3095",
                    BegVOID = "V001",
                    EndVOID = "V003",
                    BegRETURN = "R001",
                    EndRETURN = "R002",
                    LastORNumber = 125,
                    PaxCount = 45,
                    AvgTrasaction = 150.00m,
                    DineIn = 1000.00m,
                    TakeOut = 1200.00m,
                    TotalDiscount = 50.00m,
                    CashInDrawer = 3000.00m,
                    Cheque = 0.00m,
                    CreditCard = 500.00m,
                    GiftCertificate = 100.00m,
                    TotalSales = 2800.00m,
                    TotalVATableSales = 2464.00m,
                    TotalVATAmount = 336.00m,
                    TotalVATExemptSales = 0.00m,
                    TotalZeroRatedSales = 0.00m,
                    TotalNetSales = 2700.00m,
                    TotalVoid = 150.00m,
                    TotalReturn = 50.00m,
                    TotalVoidAdjustment = 10.00m,
                    TotalReturnAdjustment = 5.00m,
                    TotalCashInDrawer = 3000.00m,
                    TotalCheque = 0.00m,
                    TotalCreditCard = 500.00m,
                    TotalGiftCertificate = 100.00m,
                    TotalPaymentsReceived = 3600.00m,
                    TotalShortOver = 0.00m,
                    TotalNumVoid = 2,
                    ZReset = 1,
                    SCDiscount = 25.00m,
                    PWDDiscount = 25.00m,
                    PresentAccumulatedSales = 8000.00m,
                    PreviousAccumulatedSales = 7200.00m,
                    SalesForTheDay = 15200.00m
                },
                new SalesHistory
                {
                    ReceiptNumber = "R002",
                    CrewMemberId = 2, // Only set the CrewMemberId

                    Name = "Jollibee Gen. Maxilom",
                    Address = "1234 Jollibee St., Cebu City",
                    TIN = "123-456-789-000",
                    MIN = "123456789",
                    CashierName = "John Dela Cruz",
                    StoreCode = "JB-01",
                    InvoiceType = "Sales Invoice",
                    SNumber = "1234567890-02",
                    StartReportDate = DateTimeOffset.Now.Date.AddDays(-4),
                    EndReportDate = DateTimeOffset.Now.Date.AddDays(-1),
                    StartTime = DateTimeOffset.Now.Date.AddDays(-1),
                    EndTime = DateTimeOffset.Now.Date.AddDays(-1),
                    BegSI = "JB3085",
                    EndSI = "JB3090",
                    BegVOID = "V004",
                    EndVOID = "V005",
                    BegRETURN = "R003",
                    EndRETURN = "R004",
                    LastORNumber = 124,
                    PaxCount = 38,
                    AvgTrasaction = 120.00m,
                    DineIn = 900.00m,
                    TakeOut = 800.00m,
                    TotalDiscount = 20.00m,
                    CashInDrawer = 2000.00m,
                    Cheque = 0.00m,
                    CreditCard = 200.00m,
                    GiftCertificate = 50.00m,
                    TotalSales = 1950.00m,
                    TotalVATableSales = 1716.00m,
                    TotalVATAmount = 234.00m,
                    TotalVATExemptSales = 0.00m,
                    TotalZeroRatedSales = 0.00m,
                    TotalNetSales = 1800.00m,
                    TotalVoid = 100.00m,
                    TotalReturn = 30.00m,
                    TotalVoidAdjustment = 5.00m,
                    TotalReturnAdjustment = 3.00m,
                    TotalCashInDrawer = 2000.00m,
                    TotalCheque = 0.00m,
                    TotalCreditCard = 200.00m,
                    TotalGiftCertificate = 50.00m,
                    TotalPaymentsReceived = 2500.00m,
                    TotalShortOver = 0.00m,
                    TotalNumVoid = 1,
                    ZReset = 1,
                    SCDiscount = 10.00m,
                    PWDDiscount = 10.00m,
                    PresentAccumulatedSales = 7000.00m,
                    PreviousAccumulatedSales = 6500.00m,
                    SalesForTheDay = 13500.00m
                 }
            };



            // Initialize the observable collection with all data
            salesHistoryList = new ObservableCollection<SalesHistory>(allSalesHistory);

            // Set default selected date to today
            selectedDate = DateTime.Today;
        }

        partial void OnSelectedDateChanged(DateTimeOffset value)
        {
            _ = LoadSalesHistoryByDateAsync(value);
        }

        private async Task LoadSalesHistoryByDateAsync(DateTimeOffset date)
        {
            var filteredRecords = allSalesHistory
        .Where(x => x.StartReportDate.Date == date.Date)
        .ToList();

            if (!filteredRecords.Any())
            {
                filteredRecords.Add(new SalesHistory
                {
                    ReceiptNumber = "No Display receipt",
                    Name = "No matching record",
                    StartReportDate = date,
                    StartTime = date
                });

                // Show the message box here (needs a view reference)
                var box = MessageBoxManager
                    .GetMessageBoxStandard("No Data Found", $"No sales records found for {date:MMMM dd, yyyy}.", ButtonEnum.Ok);
                await box.ShowAsync();
            }

            SalesHistoryList = new ObservableCollection<SalesHistory>(filteredRecords);
        }

        partial void OnSearchReceiptNumberChanged(string value)
        {
            FilterSalesHistoryByReceipt(value);
        }

        private void FilterSalesHistoryByReceipt(string receiptNumber)
        {
            if (string.IsNullOrWhiteSpace(receiptNumber))
            {
                SalesHistoryList = new ObservableCollection<SalesHistory>(allSalesHistory);
                return;
            }

            var filtered = allSalesHistory
                .Where(x => x.ReceiptNumber.Contains(receiptNumber.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            SalesHistoryList = new ObservableCollection<SalesHistory>(filtered);
        }


        // This method is automatically called when the SelectedDate property changes
        //partial void OnSelectedDateChanged(DateTimeOffset value)
        //{
        //    // Filter all sales records by matching StartReportDate (compare date parts only)
        //    var filteredRecords = allSalesHistory
        //        .Where(x => x.StartReportDate.Date == value.Date)
        //        .ToList();

        //    // If no records found, add a dummy record to display a "date not found" message.
        //    if (!filteredRecords.Any())
        //    {
        //        filteredRecords.Add(new SalesHistory
        //        {
        //            ReceiptNumber = "No Display receipt",
        //            Name = "No matching record",
        //            // You can leave other properties empty or set them as needed.
        //            StartReportDate = value,
        //            StartTime = value
        //        });
        //    }

        //    // Update the observable collection bound to the DataGrid.
        //    SalesHistoryList = new ObservableCollection<SalesHistory>(filteredRecords);
        //}


    }
}
