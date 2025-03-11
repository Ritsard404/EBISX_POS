using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using EBISX_POS.Services.Transaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            LoadTransactionLogsCommand = new AsyncRelayCommand(LoadTransactionLogsAsync);
            ReprintInvoiceCommand = new AsyncRelayCommand(ReprintInvoiceAsync);

            // Load initial logs.
            LoadTransactionLogsCommand.Execute(null);
        }

        public IAsyncRelayCommand LoadTransactionLogsCommand { get; }
        public IAsyncRelayCommand ReprintInvoiceCommand { get; }

        [ObservableProperty]
        private ObservableCollection<TransactionLog> transactionLogs = new();

        // New property to hold the clicked/selected transaction.
        [ObservableProperty]
        private TransactionLog selectedTransactionLog;

        // This method is called automatically when the selectedTransactionLog property changes.
        partial void OnSelectedTransactionLogChanged(TransactionLog value)
        {
            if (value != null)
            {
                // Here you can execute any action when a row is clicked.
                // For instance, printing the transaction details or opening a detailed view.
                Debug.WriteLine($"Row clicked: Transaction ID = {value.TransactionId}");
                // You can also trigger another command if needed.
            }
        }

        private async Task LoadTransactionLogsAsync()
        {
            var logs = await _transactionLogService.GetTransactionLogsAsync();
            TransactionLogs.Clear();
            foreach (var log in logs)
            {
                TransactionLogs.Add(log);
            }
        }

        private async Task ReprintInvoiceAsync()
        {
            // Create a new transaction log entry for the reprint action.
            var newLog = new TransactionLog
            {
                LogId = Guid.NewGuid().ToString(),
                TransactionId = "TXN" + new Random().Next(1000, 9999),
                LogDate = DateTime.Now,
                LogTime = DateTime.Now.TimeOfDay,
                //Cashier = "Current Cashier", // You can bind this to the current user.
                TotalAmount = 0m,            // Update if a fee applies or leave as 0.
                Action = "Reprinted",
                Details = "Invoice receipt reprinted upon customer request."
            };

            TransactionLogs.Add(newLog);
            Debug.WriteLine($"Reprint Invoice command executed and log added. Total logs now: {TransactionLogs.Count}");
            await Task.CompletedTask;
        }


    }
}
