using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using EBISX_POS.Services.Transaction;
using EBISX_POS.ViewModels.Manager;

namespace EBISX_POS.Views {

    public partial class TransactionLogView : UserControl
    {
        public TransactionLogView()
        {
            InitializeComponent();
            // Instantiate the service and create a view model instance.
            ITransactionLogService transactionLogService = new TransactionLogService();
            DataContext = new TransactionLogViewModel(transactionLogService);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

};