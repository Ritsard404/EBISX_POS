using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using System.Linq;

namespace EBISX_POS.Views
{
    public partial class OrderItemEditWindow : Window
    {
        public OrderItemEditWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            // Close the current window
            this.Close();
        }

        private async void VoidButton_Click(object sender, RoutedEventArgs e)
        {

            // Get the view model from DataContext
            var viewModel = this.DataContext as OrderItemEditWindowViewModel;
            if (viewModel == null)
                return;

            // Retrieve the order item from the view model
            var orderItem = viewModel.OrderItem;

            var box = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentHeader = $"Void Order",
                    ContentMessage = "Please ask the manager to swipe.",
                    ButtonDefinitions = ButtonEnum.OkCancel, // Defines the available buttons
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Width = 400,
                    ShowInCenter = true,
                    Icon= MsBox.Avalonia.Enums.Icon.Warning
                });

            var result = await box.ShowAsPopupAsync(this);
            switch (result)
            {
                case ButtonResult.Ok:
                    OrderState.VoidCurrentOrder(orderItem);
                    Close();
                    return;
                case ButtonResult.Cancel:
                    return;
                default:
                    return;
            }
        }
    }
};