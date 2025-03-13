using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels;
using System.Collections.Generic;

namespace EBISX_POS.Views {

    public partial class CustomerInvoiceReceipt : Window
    {
        public CustomerInvoiceReceipt()
        {
            InitializeComponent();
        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //var viewModel = DataContext as CustomerInvoiceReceiptViewModel;
            //if (viewModel != null)
            //{
            //    var saveFileDialog = new SaveFileDialog
            //    {
            //        DefaultExtension = "txt",
            //        Filters = new List<FileDialogFilter>
            //        {
            //            new FileDialogFilter { Name = "Text Files", Extensions = { "txt" } }
            //        }
            //    };

            //    var result = saveFileDialog.ShowAsync(this).Result;
            //    if (result != null)
            //    {
            //        viewModel.PrintToTextFile(result);
            //    }
            //}
        }
    }

};