using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels;

namespace EBISX_POS.Views {

    public partial class DailySalesReportView : Window
    {
        public DailySalesReportView()
        {
            InitializeComponent();
            DataContext = new InvoiceReceiptViewModel();

        }
        private void Report_Print_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

        }
    }

};