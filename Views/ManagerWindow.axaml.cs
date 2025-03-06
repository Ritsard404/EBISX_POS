using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EBISX_POS.Views { 

public partial class ManagerWindow : Window
{
        public ManagerWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SummaryReport_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //var reportWindow = new ReportSummaryView();
            //reportWindow.Show();
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
    }

};