using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels.Manager;
using System;

namespace EBISX_POS.Views
{
    public partial class TransactionLogWindow : Window
    {
        public TransactionLogWindow()
        {
            InitializeComponent();
            // Instantiate the service and create a view model instance.
            DataContext = new TransactionViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
};