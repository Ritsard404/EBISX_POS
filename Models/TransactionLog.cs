using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Models
{
    public class TransactionLog
    {
        public string LogId { get; set; }
        public string TransactionId { get; set; }
        public DateTime LogDate { get; set; }
        public string Action { get; set; }  // e.g., "Created", "Reprinted"
        public string Details { get; set; }
        public string Cashier { get; set; } // Added property
        public TimeSpan LogTime { get; set; } // Added property
        public decimal TotalAmount { get; set; } // Added property
    }
}
