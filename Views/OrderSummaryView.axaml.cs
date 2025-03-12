using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia;
using System.Diagnostics;
using MsBox.Avalonia.Enums;
using Avalonia.Controls.ApplicationLifetimes;

namespace EBISX_POS.Views
{
    public partial class OrderSummaryView : UserControl
    {
        public OrderSummaryView()
        {
            InitializeComponent();
            DataContext = new OrderSummaryViewModel();
        }

        private async void EditOrder_Button(object? sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton && clickedButton.DataContext is OrderItemState SelectedCurrentOrderItem)
            {
                var detailsWindow = new OrderItemEditWindow()
                {
                    DataContext = new OrderItemEditWindowViewModel(SelectedCurrentOrderItem)
                };

                Debug.WriteLine("EditOrder_Button: " + SelectedCurrentOrderItem.DisplaySubOrders.Count);


                await detailsWindow.ShowDialog((Window)this.VisualRoot);
            }
        }

        private async void VoidCurrentOrder_Button(object? sender, RoutedEventArgs e)
        {
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var owner = lifetime?.MainWindow;

            var box = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentHeader = "Void Order",
                    ContentMessage = "Please ask the manager to swipe.",
                    ButtonDefinitions = ButtonEnum.OkCancel,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Width = 300,
                    ShowInCenter = true,
                    Icon = Icon.Warning
                });

            var result = await box.ShowAsPopupAsync(owner);

            switch (result)
            {
                case ButtonResult.Ok:
                    OrderState.CurrentOrderItem = new OrderItemState();
                    OrderState.CurrentOrderItem.RefreshDisplaySubOrders();
                    return;
                case ButtonResult.Cancel:
                    Debug.WriteLine("VoidCurrentOrder_Button: Order voided");
                    return;
                default:
                    return;
            }

        }
    }
};
