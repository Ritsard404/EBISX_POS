using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.State;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using System.Diagnostics;

namespace EBISX_POS.Views
{
    public partial class PromoCodeWindow : Window
    {
        public PromoCodeWindow()
        {
            InitializeComponent();
            PromoCodeTextBox = this.FindControl<TextBox>("PromoCodeTextBox");
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private async void ApplyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PromoCodeTextBox?.Text)) // Proper null and empty check
            {
                var emptyCode = await MessageBoxManager.GetMessageBoxStandard(
                    new MessageBoxStandardParams
                    {
                        ContentHeader = "Error",
                        ContentMessage = "Promo Code cannot be empty!",
                        ButtonDefinitions = ButtonEnum.Ok,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        CanResize = false,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        Width = 400,
                        ShowInCenter = true,
                        Icon = MsBox.Avalonia.Enums.Icon.Error
                    }).ShowAsPopupAsync(this);
                if (emptyCode == ButtonResult.Ok)
                {
                    return;
                }
            }

            var box = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentHeader = $"Promo Code: {PromoCodeTextBox.Text.Trim()}",
                    ContentMessage = "Promo Code Exist",
                    ButtonDefinitions = ButtonEnum.OkCancel,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Width = 400,
                    ShowInCenter = true,
                    Icon = MsBox.Avalonia.Enums.Icon.Warning
                });

            var result = await box.ShowAsPopupAsync(this);
            if (result == ButtonResult.Ok)
            {
                //TenderState.tenderOrder.HasPromoDiscount = !TenderState.tenderOrder.HasPromoDiscount;
                TenderState.tenderOrder.PromoDiscountAmount = 100;
                Close();
            }
            else if (result == ButtonResult.Cancel)
            {
                Close();
                return;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
};