using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS
{
    public class SalesReport
    {
        public required  string Reciepts { get; set; }
        public required  string SearchedInvoice { get; set; }
        public required  string DailySalesReport { get; set; }
        public required  string XInvoiceReport { get; set; }
        public required  string ZInvoiceReport { get; set; }
        public required  string CashTrackReport { get; set; }
        public required  string TransactionLogs { get; set; }
    }
}
