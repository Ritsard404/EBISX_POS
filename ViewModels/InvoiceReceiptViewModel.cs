using EBISX_POS.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EBISX_POS.ViewModels
{
    public class InvoiceReceiptViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<InvoiceReceipt> _reports;
        public ObservableCollection<InvoiceReceipt> Reports
        {
            get => _reports;
            set
            {
                _reports = value;
                OnPropertyChanged(nameof(Reports));
            }
        }

        public InvoiceReceiptViewModel()
        {
            Reports = new ObservableCollection<InvoiceReceipt>
            {
                new InvoiceReceipt
                {
                    BusinessName = "TESSA’S RESTAURANT",
                    Operator = "CBR Foods, Inc.",
                    Address = "2nd Floor MTNL Bldg. Quen's Ave., Brgy. Poblacion Sur, Paniqui, Tarlac",
                    VATRegTIN = "321-654-987-00000",
                    MIN = "0987654321",
                    SerialNumber = "1234567890-01",
                    ReportType = "X-READING REPORT",

                    ReportDate = "March 15, 2021",
                    ReportTime = "11:30 AM",
                    StartDateTime = "03/15/21 9:00 AM",
                    EndDateTime = "03/15/21 11:29 AM",
                    Cashier = "Gina Ramos",

                    // Payment Summary
                    BeginningSI = 0000000001,
                    EndingSI = 00000007,
                    BeginningVOID = 0000000001,
                    EndingVOID = 0000000001,
                    OpeningFund = 0.00m,
                    CashReceived = 718.40m,
                    ChequeReceived = 30.00m,
                    CreditCardReceived = 50.00m,
                    TotalPayments = 798.40m,

                        // Sales Summary
                    PresentAccumulatedSales = 1206.00m,
                    PreviousAccumulatedSales = 0.00m,
                    SalesForTheDay = 1206.00m,


                    //Sales Sales Breakdown
                    ZeroRatedSales = 0.00m,
                    VatExemptSales = 0.00m,
                    VatableSales = 718.40m,
                    VatAmount = 86.21m,
                  
                    // Transaction Summary
                    CashInDrawer = 720.00m,
                    WithdrawalAmount = 0.00m,
                    ShortOver = 1.60m
                },

                //new ReportModel
                //{
                //    BusinessName = "NICOLE'S SUPERMARKET",
                //    Operator = "Facunla Enterprise Inc.",
                //    Address = "Ground Floor Jade Bldg., Jennalyn Ave., Brgy. Abogado, Paniqui, Tarlac",
                //    VATRegTIN = "123-456-789-00000",
                //    MIN = "1234567890",
                //    SerialNumber = "0987654321-11",
                //    ReportType = "Z-READING REPORT",

                //    ReportDate = "March 15, 2021",
                //    ReportTime = "11:30 AM",
                //    StartDateTime = "03/15/21 9:00 AM",
                //    EndDateTime = "03/15/21 11:29 AM",

                //    // Sales Summary
                //    PresentAccumulatedSales = 1206.00m,
                //    PreviousAccumulatedSales = 0.00m,
                //    SalesForTheDay = 1206.00m,

                //    // Sales Breakdown
                //    VatableSales = 533.74m,
                //    VatAmount = 64.06m,
                //    VatExemptSales = 359.43m,
                //    ZeroRatedSales = 0.00m,

                //    // Transaction Summary
                //    CashInDrawer = 635.00m,
                //    ChequeReceived = 100.00m,
                //    CreditCardReceived = 100.00m,
                //    GiftCertificate = 100.00m,
                //    TotalPayments = 934.14m,
                //    ShortOver = 0.86m
                //}
            };
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
