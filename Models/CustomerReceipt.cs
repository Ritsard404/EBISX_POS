using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Models
{
    public class CustomerReceipt
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string TIN { get; set; }
        public string MIN { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string Cashier { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public double SubTotal { get; set; }
        public double TotalDue { get; set; }
        public double Cash { get; set; }
        public double Change { get; set; }

        public CustomerReceipt()
        {
            Items = new List<InvoiceItem>();
        }
    }

    public class InvoiceItem
    {
        public int Qty { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }
}
