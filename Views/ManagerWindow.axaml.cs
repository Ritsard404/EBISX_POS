using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Avalonia.Interactivity;



namespace EBISX_POS.Views { 

public partial class ManagerWindow : Window
{
        private readonly IServiceProvider _serviceProvider;
        public ManagerWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            DataContext = this;
            _serviceProvider = serviceProvider;
        }

        private void SummaryReport_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var reportWindow = _serviceProvider.GetRequiredService<DailySalesReportView>();
            reportWindow.Show();

            //var invoiceReceipt = new CustomerInvoiceReceipt();
            //invoiceReceipt.Show();
        }

        private void CrewLogin(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //var CrewlogWindow = new CrewLogEntryView();
            //CrewlogWindow.Show();
        }

        private void TransactionLog(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var transactionLogWindow = new TransactionLogWindow();
            transactionLogWindow.Show();
        }

        private async  void Cash_Track_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                GenerateFile(sender, e);
                await MessageBoxManager
                    .GetMessageBoxStandard("Info", "Daily Sales Report has been generated.", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
            }
            catch (Exception ex)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", ex.Message, ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
            }
        }

        private void GenerateFile(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

};