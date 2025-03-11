using EBISX_POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Services.Transaction
{
    public class TransactionLogService : ITransactionLogService
    {
        public async Task<IEnumerable<TransactionLog>> GetTransactionLogsAsync()
        {
            // Simulate asynchronous data retrieval (replace with actual data source).
            await Task.Delay(500);
            return new List<TransactionLog>
            {
                new TransactionLog
                {
                    LogId = "LOG001",
                    TransactionId = "TXN1001",
                    LogDate = DateTime.Now,
                    LogTime = DateTime.Now.TimeOfDay,
                    //Cashier = "John Doe",
                    TotalAmount = 100.00m,
                    Action = "Created",
                    Details = "Transaction created successfully."
                },
                new TransactionLog
                {
                  
                    LogId = "LOG002",
                    TransactionId = "TXN1002",
                    LogDate = DateTime.Now.AddMinutes(-15),
                    LogTime = DateTime.Now.AddMinutes(-15).TimeOfDay,
                    //Cashier = "Jane Smith",
                    TotalAmount = 50.00m,
                    Action = "Reprinted",
                    Details = "Invoice reprinted upon customer request."
            
            
                }
            };
        }
    }
}
