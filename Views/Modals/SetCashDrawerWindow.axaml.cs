using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EBISX_POS.Services;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Text.RegularExpressions;

namespace EBISX_POS.Views
{
    public partial class SetCashDrawerWindow : Window
    {
        private string _cashDrawer;

        public SetCashDrawerWindow(string cashDrawer)
        {
            InitializeComponent();
            _cashDrawer = cashDrawer;

            StartButton.Content = _cashDrawer switch
            {
                "Cash-In" => "Start",
                "Cash-Out" => "Set Drawer",
                "Withdraw" => "Withdraw",
                "Returned" => "Refund",
                _ => "Submit"
            };

            ManagerEmail.IsVisible = _cashDrawer == "Withdraw" || _cashDrawer == "Returned";

            if (_cashDrawer == "Returned")
                CashInDrawer.Watermark = "Enter Invoice ID";

            CashInDrawer.AddHandler(TextInputEvent, AmountTextBox_OnTextInput, RoutingStrategies.Tunnel);
        }

        private void AmountTextBox_OnTextInput(object sender, TextInputEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var currentText = textBox.Text ?? "";
                var newText = currentText.Insert(textBox.CaretIndex, e.Text);

                if (!Regex.IsMatch(newText, @"^\d*(\.\d{0,2})?$"))
                {
                    e.Handled = true;
                }
            }
        }

        private async void Start_Click(object? sender, RoutedEventArgs e)
        {
            var authService = App.Current.Services.GetRequiredService<AuthService>();
            var orderService = App.Current.Services.GetRequiredService<OrderService>();

            var input = this.FindControl<TextBox>("CashInDrawer")?.Text;
            var managerEmail = this.FindControl<TextBox>("ManagerEmail")?.Text;

            if (string.IsNullOrWhiteSpace(input))
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Invalid Input", "Input field is required.", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
                return;
            }

            if (_cashDrawer == "Returned")
            {
                if (!long.TryParse(input, out var orderId))
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Invalid Input", "Please enter a valid Order ID.", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                    return;
                }

                if (string.IsNullOrWhiteSpace(managerEmail))
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Invalid Input", "Manager email is required.", ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);
                    return;
                }

                StartButton.IsEnabled = false;

                var (isSuccess, message) = await orderService.RefundOrder(managerEmail, orderId);

                if (!isSuccess)
                {
                    await MessageBoxManager
                        .GetMessageBoxStandard("Error", message, ButtonEnum.Ok)
                        .ShowAsPopupAsync(this);

                    StartButton.IsEnabled = true;
                    return;
                }

                Close();
                return;
            }

            // Default: Cash-In / Cash-Out / Withdraw
            if (!decimal.TryParse(input, out var amount) || amount < 1000)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Invalid Input", "Please enter a valid amount.", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
                return;
            }

            if (_cashDrawer == "Withdraw" && string.IsNullOrWhiteSpace(managerEmail))
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Invalid Input", "Manager email is required.", ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
                return;
            }

            StartButton.IsEnabled = false;

            var (success, msg) = _cashDrawer switch
            {
                "Cash-In" => await authService.SetCashInDrawer(amount),
                "Cash-Out" => await authService.SetCashOutDrawer(amount),
                "Withdraw" => await authService.CashWithdrawDrawer(managerEmail!, amount),
                _ => (false, "Invalid operation")
            };

            if (!success)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", msg, ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);

                StartButton.IsEnabled = true;
                return;
            }

            Close();
        }
    }
}
