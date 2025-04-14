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
        private bool _isCashInDrawer;
        public SetCashDrawerWindow(bool isCashInDrawer)
        {
            InitializeComponent();
            _isCashInDrawer = isCashInDrawer;

            StartButton.Content = isCashInDrawer ? "Start" : "Set Drawer";

            CashInDrawer.AddHandler(TextInputEvent, AmountTextBox_OnTextInput, RoutingStrategies.Tunnel);
        }

        // Event handler to ensure the text is a valid decimal with up to 3 decimal places
        private void AmountTextBox_OnTextInput(object sender, TextInputEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Use an empty string if Text is null to prevent NullReferenceException
                var currentText = textBox.Text ?? "";
                var newText = currentText.Insert(textBox.CaretIndex, e.Text);

                // Regex: any number of digits, optional decimal point with up to 3 digits
                if (!Regex.IsMatch(newText, @"^\d*(\.\d{0,2})?$"))
                {
                    e.Handled = true;
                }
            }
        }

        private async void Start_Click(object? sender, RoutedEventArgs e)
        {
            var authService = App.Current.Services.GetRequiredService<AuthService>();

            var input = this.FindControl<TextBox>("CashInDrawer")?.Text;


            if (string.IsNullOrWhiteSpace(input) ||
                !decimal.TryParse(input, out var amount) || amount < 1000)
            {
                // Show an error and abort
                await MessageBoxManager
                    .GetMessageBoxStandard(
                        "Invalid Input",
                        "Please enter a valid number for the cash drawer.",
                        ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);
                return;
            }
            StartButton.IsEnabled = false;

            var (isSuccess, message) = _isCashInDrawer ? await authService.SetCashInDrawer(amount)
                : await authService.SetCashOutDrawer(amount);

            if (!isSuccess)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard(
                "Error",
                        message,
                        ButtonEnum.Ok)
                    .ShowAsPopupAsync(this);

                StartButton.IsEnabled = true;
                return;
            }

            Close();
        }
    }
};
