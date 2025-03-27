using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EBISX_POS.ViewModels
{
    public partial class CustomerInvoiceReceiptViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _businessName = "Jollibee Gen. Maxilom";

        [ObservableProperty]
        private string _address = "1234 Jollibee St., Cebu City";

        [ObservableProperty]
        private string _contactNumber = "123-456-7890";

        [ObservableProperty]
        private string _tin = "123-456-789-000";

        [ObservableProperty]
        private string _min = "123456789";

        [ObservableProperty]
        private string _invoiceNumber = "JB3095";

        [ObservableProperty]
        private string _invoiceType = "Sales Invoice";

        [ObservableProperty]
        private DateTime _invoiceDate = DateTime.Now;

        [ObservableProperty]
        private string _serialNumber = "1234567890-01";


        [ObservableProperty]
        private string _cashier = "Nashley Taba";

        [ObservableProperty]
        private decimal _subTotal = 240.00m;

        [ObservableProperty]
        private decimal _totalAmount = 240.00m;

        [ObservableProperty]
        private decimal _cashReceived = 300.00m;

        [ObservableProperty]
        private decimal _change = 60.00m;

        [ObservableProperty]
        private decimal _vatableSales = 300.00m;

        [ObservableProperty]
        private decimal _vatExemptSales = 23.50m;

        [ObservableProperty]
        private decimal _vatZeroRatedSales = 0.00m;

        [ObservableProperty]
        private decimal _totalVat = 53.64m;

        [ObservableProperty]
        private string _vatRegTin = "123-456-789-0";

        public ObservableCollection<InvoiceItem> InvoiceItems { get; set; } = new()
        {
            new InvoiceItem { Quantity = 1, Description = "Chicken Joy", Price = 100.00m },
            new InvoiceItem { Quantity = 1, Description = "Spaghetti", Price = 50.00m },
            new InvoiceItem { Quantity = 1, Description = "Burger Steak", Price = 60.00m },
            new InvoiceItem { Quantity = 1, Description = "Fries", Price = 30.00m },
        };
    }

    public class InvoiceItem
    {
        public int Quantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Amount => Quantity * Price;
    }
}
