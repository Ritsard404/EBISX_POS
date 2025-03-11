using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels.Manager;

namespace EBISX_POS.Views
{
    public partial class TransactionView : UserControl
    {
        public TransactionView()
        {
            InitializeComponent();
            DataContext = new TransactionViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            // Cast DataContext to TransactionViewModel
            var viewModel = DataContext as TransactionViewModel;
            if (viewModel != null)
            {
                // Bind the ViewTransactionCommand to the button
                var button = this.FindControl<Button>("ViewButton");
                if (button != null)
                {
                    button.Command = viewModel.ViewTransactionCommand;
                }
            }
        }
    }
}