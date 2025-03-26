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
using System.Linq;

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

        private async void PromoAndCouponDiscount_Click(object? sender, RoutedEventArgs e)
        {
            // Prevent multiple discounts on the same order.
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

            if (sender is Button btn)
            {
                string discountType = GetDiscountType(btn);

                // Handle actions based on the discount type.
                switch (discountType)
                {
                    case "PROMO":
                        {
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
                            }
                            break;
                        }
                    case "COUPON":
                        {
                            // TODO: Implement coupon discount handling.
                            break;
                        }
                    default:
                        {
                            // Optionally, handle unexpected discount types.
                            break;
                        }
                }
            }
        }

        private string GetDiscountType(ContentControl control)
        {
            // Check if the Content is a TextBlock.
            if (control.Content is TextBlock textBlock)
            {
                return textBlock.Text;
            }
            // If the Content is a Panel (e.g., a StackPanel), attempt to get the first TextBlock.
            else if (control.Content is Panel panel)
            {
                var childTextBlock = panel.Children.OfType<TextBlock>().FirstOrDefault();
                return childTextBlock?.Text ?? string.Empty;
            }
            // Fallback: return the Content's string representation.
            return control.Content?.ToString() ?? string.Empty;
        }



    }
};
