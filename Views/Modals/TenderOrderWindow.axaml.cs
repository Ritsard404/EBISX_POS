using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.API.Models;
using EBISX_POS.Models;
using EBISX_POS.State;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Diagnostics;
using EBISX_POS.Services;
using Microsoft.Extensions.DependencyInjection;
using EBISX_POS.API.Services.DTO.Order;

namespace EBISX_POS.Views
{
    public partial class TenderOrderWindow : Window
    {
        public TenderOrderWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void EnterButton_Click(object? sender, RoutedEventArgs e)
        {
            var orderService = App.Current.Services.GetRequiredService<OrderService>();

            // Check if the tendered amount is sufficient
            if (TenderState.tenderOrder.TenderAmount >= TenderState.tenderOrder.AmountDue)
            {
                var finalOrder = new FinalizeOrderDTO()
                {
                    TotalAmount = TenderState.tenderOrder.TotalAmount,
                    CashTendered = TenderState.tenderOrder.TenderAmount,
                    OrderType = TenderState.tenderOrder.OrderType
                };

                await orderService.FinalizeOrder(finalOrder);
                OrderState.CurrentOrderItem = new OrderItemState();
                OrderState.CurrentOrder.Clear();
                OrderState.CurrentOrderItem.RefreshDisplaySubOrders();
                TenderState.tenderOrder.Reset(); Debug.WriteLine("[EnterButton_Click] Order finalized and order state reset. Closing window.");

                Close();
                return;
            }

            await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentHeader = "Insufficient Tender Amount",
                ContentMessage = "Input appropriate tender amount.",
                ButtonDefinitions = ButtonEnum.Ok,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                Width = 400,
                ShowInCenter = true,
                Icon = MsBox.Avalonia.Enums.Icon.Warning
            }).ShowAsPopupAsync(this);
        }

        //private async void PwdScDiscount_Click(object? sender, RoutedEventArgs e)
        //{
        //    if (TenderState.tenderOrder.HasOrderDiscount)
        //    {
        //        await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        //        {
        //            ContentHeader = "Discounted Already",
        //            ContentMessage = "This order is already discounted.",
        //            ButtonDefinitions = ButtonEnum.Ok,
        //            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        //            CanResize = false,
        //            SizeToContent = SizeToContent.WidthAndHeight,
        //            Width = 400,
        //            ShowInCenter = true,
        //            Icon = MsBox.Avalonia.Enums.Icon.Warning
        //        }).ShowAsPopupAsync(this);
        //        return;
        //    }

        //    var result = await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        //    {
        //        ContentHeader = "PWD/SC Discount",
        //        ContentMessage = "Please ask the manager to swipe.",
        //        ButtonDefinitions = ButtonEnum.OkCancel,
        //        WindowStartupLocation = WindowStartupLocation.CenterOwner,
        //        CanResize = false,
        //        SizeToContent = SizeToContent.WidthAndHeight,
        //        Width = 400,
        //        ShowInCenter = true,
        //        Icon = MsBox.Avalonia.Enums.Icon.Warning
        //    }).ShowAsPopupAsync(this);

        //    if (result == ButtonResult.Ok)
        //        TenderState.tenderOrder.HasPwdScDiscount = !TenderState.tenderOrder.HasPwdScDiscount;
        //}

        private async void PromoDiscount_Click(object? sender, RoutedEventArgs e)
        {
            if (TenderState.tenderOrder.HasScDiscount || TenderState.tenderOrder.HasPwdDiscount)
            {
                await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentHeader = "Discounted Already",
                    ContentMessage = "This order is already discounted.",
                    ButtonDefinitions = ButtonEnum.Ok,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Width = 400,
                    ShowInCenter = true,
                    Icon = MsBox.Avalonia.Enums.Icon.Warning
                }).ShowAsPopupAsync(this);
                return;
            }

            var result = await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentHeader = "Promo Discount",
                ContentMessage = "Please ask the manager to swipe.",
                ButtonDefinitions = ButtonEnum.OkCancel,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                Width = 400,
                ShowInCenter = true,
                Icon = MsBox.Avalonia.Enums.Icon.Warning
            }).ShowAsPopupAsync(this);

            if (result == ButtonResult.Ok)
            {

                var promoWindow = new PromoCodeWindow();
                await promoWindow.ShowDialog((Window)this.VisualRoot);
                return;
            }

            if (result == ButtonResult.Cancel)
                return;



        }

    }
};
