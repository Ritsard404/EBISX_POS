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
        public TimeSpan LogTime { get; set; }
        public decimal TotalAmount { get; set; }

        // Foreign key for CrewMember (cashier)
        public int CrewMemberId { get; set; }
        // Navigation property to CrewMember
        public CrewMember CrewMember { get; set; }

        // Constructor similar to Person class
        public TransactionLog(string logId, string transactionId, DateTime logDate, string action, string details, TimeSpan logTime, decimal totalAmount, int crewMemberId, CrewMember crewMember)
        {
            LogId = logId;
            TransactionId = transactionId;
            LogDate = logDate;
            Action = action;
            Details = details;
            LogTime = logTime;
            TotalAmount = totalAmount;
            CrewMemberId = crewMemberId;
            CrewMember = crewMember;
        }

        // Parameterless constructor for initialization without parameters
        public TransactionLog() { }

    }
}
