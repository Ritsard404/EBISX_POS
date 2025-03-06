using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using EBISX_POS.Services.Transaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels.Manager
{
    public partial class TransactionLogViewModel : ObservableObject
    {
        private readonly ITransactionLogService _transactionLogService;

        public TransactionLogViewModel(ITransactionLogService transactionLogService)
        {
            _transactionLogService = transactionLogService;
            // Automatically load logs on initialization (optional)
            //LoadTransactionLogsCommand.Execute(null);
        }

        // Automatically generated property with change notifications.
        [ObservableProperty]
        private ObservableCollection<TransactionLog> transactionLogs = new();

        // RelayCommand to load transaction logs asynchronously.
        [RelayCommand]
        private async Task LoadTransactionLogsAsync()
        {
            var logs = await _transactionLogService.GetTransactionLogsAsync();
            TransactionLogs.Clear();
            foreach (var log in logs)
            {
                TransactionLogs.Add(log);
            }
        }

    }
}
