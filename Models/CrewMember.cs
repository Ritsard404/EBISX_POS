using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Models
{
    public class CrewMember
    {
        public int CrewMemberId { get; set; }   // Primary key
        public string Name { get; set; }
        // Navigation property for TransactionLogs (optional if using an ORM)
        public List<TransactionLog> TransactionLogs { get; set; } = new();
    }
}
