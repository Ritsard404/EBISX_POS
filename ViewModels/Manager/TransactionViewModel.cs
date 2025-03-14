using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels.Manager
{
    public partial class TransactionViewModel : ObservableObject
    {
        // Observable collection for transaction logs
        [ObservableProperty]
        private ObservableCollection<TransactionLog> transactionLogs = new();

        // Selected transaction log
        [ObservableProperty]
        private TransactionLog selectedTransactionLog;

        // Command for viewing transaction details
        public IAsyncRelayCommand<TransactionLog> ViewTransactionCommand { get; }

        public TransactionViewModel()
        {
            // Initialize sample data within the property
            TransactionLogs = new ObservableCollection<TransactionLog>
            {
                new TransactionLog
                {
                    LogId = "1",
                    TransactionId = "TXN001",
                    LogDate = DateTime.Now,
                    LogTime = DateTime.Now.TimeOfDay,
                    TotalAmount = 1000,
                    Action = "Created",
                    Details = "Initial transaction created",
                    CrewMemberId = 1,
                    CrewMember = new CrewMember { CrewMemberId = 1, Name = "John Doe" }
                },
                new TransactionLog
                {
                    LogId = "2",
                    TransactionId = "TXN002",
                    LogDate = DateTime.Now.AddMinutes(-30),
                    LogTime = DateTime.Now.AddMinutes(-30).TimeOfDay,
                    TotalAmount = 2000,
                    Action = "Reprinted",
                    Details = "Invoice reprinted",
                    CrewMemberId = 2,
                    CrewMember = new CrewMember { CrewMemberId = 2, Name = "Jane Smith" }
                },
                new TransactionLog
                {
                    LogId = "3",
                    TransactionId = "TXN003",
                    LogDate = DateTime.Now.AddHours(-1),
                    LogTime = DateTime.Now.AddHours(-1).TimeOfDay,
                    TotalAmount = 1500,
                    Action = "Updated",
                    Details = "Transaction updated",
                    CrewMemberId = 3,
                    CrewMember = new CrewMember { CrewMemberId = 3, Name = "Alice Johnson" }
                }
            };

            Debug.WriteLine($"Loaded {TransactionLogs.Count} transactions.");

            //// Initialize ViewTransactionCommand
            //ViewTransactionCommand = new AsyncRelayCommand<TransactionLog>(ViewTransactionAsync);
        }

        // Triggered when SelectedTransactionLog changes
        //partial void OnSelectedTransactionLogChanged(TransactionLog value)
        //{
        //    if (value != null)
        //    {
        //        Debug.WriteLine($"Row clicked: Transaction ID = {value.TransactionId}");
        //    }
        //}

        // Properly defined async command method
        //private Task ViewTransactionAsync(TransactionLog transaction)
        //{
        //    if (transaction != null)
        //    {
        //        Debug.WriteLine($"ViewTransactionCommand executed for Transaction ID = {transaction.TransactionId}");
        //        // Implement further logic, e.g., navigating to a detail view.
        //    }
        //    return Task.CompletedTask;
        //}
    }
}
