using EBISX_POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Services.Transaction
{
    public interface ITransactionLogService
    {
        Task<IEnumerable<TransactionLog>> GetTransactionLogsAsync();
    }
}
