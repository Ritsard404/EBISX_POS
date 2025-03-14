using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace EBISX_POS.Views
{
    public partial class ManagerWindow : Window
    {
        private readonly IServiceProvider? _serviceProvider;

        // Constructor with IServiceProvider parameter
        public ManagerWindow(IServiceProvider serviceProvider) 
        {
            InitializeComponent();
            DataContext = this;
            _serviceProvider = serviceProvider;
        }

        private void SummaryReport_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var reportWindow = _serviceProvider?.GetRequiredService<DailySalesReportView>();
            reportWindow?.Show();
        }

        private void TransactionLog(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var transactionwindow = new TransactionView();
            transactionwindow.Show();
        }

        private async void Cash_Track_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var cashTrack = _serviceProvider?.GetRequiredService<CashTrackView>();
            if (cashTrack != null)
            {
                cashTrack.GenerateCashTrack(sender, e);
            }
        }
    }
}