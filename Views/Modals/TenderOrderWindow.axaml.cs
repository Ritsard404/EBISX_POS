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
            // Prevent applying multiple discounts to the same order.
            if (TenderState.tenderOrder.HasScDiscount || TenderState.tenderOrder.HasPwdDiscount || TenderState.tenderOrder.HasPromoDiscount)
            {
                await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentHeader = "Discount Already Applied",
                    ContentMessage = "A discount has already been applied to this order. Please complete or cancel the current order before applying another discount.",
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
                            if (OrderState.CurrentOrder.Any(d => d.CouponCode != null))
                            {
                                await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                                {
                                    ContentHeader = "Coupon Discount Detected",
                                    ContentMessage = "This order already has a coupon discount applied.",
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
                                ContentHeader = "Apply Promo Discount",
                                ContentMessage = "Manager authorization is required to apply a promo discount. Please ask the manager to swipe their card.",
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
                                var promoWindow = new DiscountCodeWindow("PROMO");
                                await promoWindow.ShowDialog((Window)this.VisualRoot);
                            }
                            break;
                        }
                    case "COUPON":
                        {
                            if (OrderState.CurrentOrder.Count(d => d.CouponCode != null) >= 3)
                            {
                                await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                                {
                                    ContentHeader = "Maximum Coupon Discount Reached",
                                    ContentMessage = "This order already has the maximum allowed coupon discounts applied (3 coupons). You cannot apply any additional coupon discounts.",
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
                                ContentHeader = "Apply Coupon Discount",
                                ContentMessage = "Manager authorization is required to apply a coupon discount. Please ask the manager to swipe their card.",
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
                                var promoWindow = new DiscountCodeWindow("COUPON");
                                await promoWindow.ShowDialog((Window)this.VisualRoot);
                            }
                            break;
                        }
                    default:
                        {
                            await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                            {
                                ContentHeader = "Unknown Discount Type",
                                ContentMessage = "The selected discount type is not recognized. Please try again or contact support.",
                                ButtonDefinitions = ButtonEnum.Ok,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                CanResize = false,
                                SizeToContent = SizeToContent.WidthAndHeight,
                                Width = 400,
                                ShowInCenter = true,
                                Icon = MsBox.Avalonia.Enums.Icon.Warning
                            }).ShowAsPopupAsync(this);
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
