using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Microsoft.Extensions.DependencyInjection;
using EBISX_POS.Services;
using EBISX_POS.ViewModels;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using EBISX_POS.State;
using EBISX_POS.Models;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Options;
using EBISX_POS.Util;
using EBISX_POS.Views.Manager;
using EBISX_POS.ViewModels.Manager;
using EBISX_POS.API.Models;
using static Avalonia.Media.Transformation.TransformOperation;
using System.Threading.Tasks;

namespace EBISX_POS.Views
{
    public partial class ManagerWindow : Window
    {
        private readonly string _cashTrackReportPath;

        private readonly IServiceProvider? _serviceProvider;
        private readonly MenuService _menuService;
        private readonly AuthService _authService;

        // Constructor with IServiceProvider parameter
        public ManagerWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            DataContext = this;

            _serviceProvider = serviceProvider;

            // fetch only what we actually need
            _menuService = serviceProvider.GetRequiredService<MenuService>();
            _authService = serviceProvider.GetRequiredService<AuthService>();
            _cashTrackReportPath = serviceProvider
                                        .GetRequiredService<IOptions<SalesReport>>()
                                        .Value.CashTrackReport;

            // cache these checks so you only call them once
            bool hasManager = !string.IsNullOrWhiteSpace(CashierState.ManagerEmail);
            bool hasCashier = !string.IsNullOrWhiteSpace(CashierState.CashierEmail);

            // show overlay & hide BackButton only if nobody is signed in
            var overlayOn = !(hasManager || hasCashier);
            ButtonOverlay.IsVisible = overlayOn;
            ButtonOverlay.IsHitTestVisible = overlayOn;

            BackButton.IsVisible = (hasManager || hasCashier);

            // enable SalesReport only for managers
            SalesReport.IsEnabled = hasManager;

            DataLayout.IsVisible = hasManager || !(hasManager || hasCashier);

            // disable LogOut for managers (since they’d go back to login instead)
            LogOut.IsEnabled = !hasManager;
        }
        public ManagerWindow() : this(App.Current.Services.GetRequiredService<IServiceProvider>())
        { }
        // This constructor is required for Avalonia to instantiate the view in XAML.

        private void ShowLoader(bool show)
        {
            LoadingOverlay.IsVisible = show;
        }

        private async void SalesReport_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //var reportWindow = _serviceProvider?.GetRequiredService<DailySalesReportView>();
            //reportWindow?.Show();
            if (!string.IsNullOrWhiteSpace(CashierState.CashierEmail))
                return;

            var swipeManager = new ManagerSwipeWindow(header: "Z Reading", message: "Please ask the manager to swipe.", ButtonName: "Swipe");
            bool isSwiped = await swipeManager.ShowDialogAsync(this);

            if (isSwiped)
            {
                ShowLoader(true);
                ReceiptPrinterUtil.PrintZReading(_serviceProvider!);
                ShowLoader(false);
            }

        }

        private void TransactionLog(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var reportWindow = _serviceProvider?.GetRequiredService<SalesHistoryWindow>();
            reportWindow?.ShowDialog(this);
        }

        private async void Cash_Track_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //var cashTrack = _serviceProvider?.GetRequiredService<CashTrackView>();
            //if (cashTrack != null)
            //{
            //    cashTrack.GenerateCashTrack(sender, e);
            //}


            ShowLoader(true);
            var reportService = App.Current.Services.GetRequiredService<ReportService>();

            var (CashInDrawer, CurrentCashDrawer) = await reportService.CashTrack();

            // Ensure the target directory exists
            if (!Directory.Exists(_cashTrackReportPath))
            {
                Directory.CreateDirectory(_cashTrackReportPath);
            }

            // Define the file path
            string fileName = $"Cash-Track-{CashierState.CashierEmail}-{DateTimeOffset.UtcNow.ToString("MMMM-dd-yyyy-HH-mm-ss")}.txt";
            string filePath = Path.Combine(_cashTrackReportPath, fileName);

            string reportContent = $@"
                ==================================
                        Cash Track Report
                ==================================
                Cash In Drawer: {CashInDrawer:C}
                Total Cash Drawer: {CurrentCashDrawer:C}
                ";

            reportContent = string.Join("\n", reportContent.Split("\n").Select(line => line.Trim()));
            File.WriteAllText(filePath, reportContent);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            ShowLoader(false);
        }

        private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var isManager = !string.IsNullOrWhiteSpace(CashierState.ManagerEmail);

            // Only reset if they were a manager
            if (isManager)
                CashierState.CashierStateReset();

            // Pick which window to open
            Window nextWindow = isManager
                ? (Window)new LogInWindow()
                : new MainWindow(_menuService, _authService)
                {
                    DataContext = new MainWindowViewModel(_menuService)
                };

            nextWindow.Show();
            Close();
        }


        private async void CashPullOut_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var setCashDrawer = new SetCashDrawerWindow("Withdraw");
            await setCashDrawer.ShowDialog(this);
        }
        private async void ManagerLog_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var managerLog = new UserLogsWindow();
            managerLog.DataContext = new UserLogsViewModel(true);
            await managerLog.ShowDialog(this);
        }
        private async void CashierLog_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var managerLog = new UserLogsWindow();
            managerLog.DataContext = new UserLogsViewModel(false);
            await managerLog.ShowDialog(this);
        }
        private async void LoadData_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                ShowLoader(true);

                if (!await NetworkHelper.IsOnlineAsync())
                {
                    await ShowMessageBoxAsync("No Internet Connection", "Please check your network and try again.");
                    return;
                }

                var (isSuccess, message) = await _authService.LoadDataAsync();

                if (!isSuccess)
                {
                    await ShowMessageBoxAsync("Load Failed",
                        string.IsNullOrWhiteSpace(message) ? "An unknown error occurred." : message);
                    return;
                }

                if (!string.IsNullOrEmpty(CashierState.ManagerEmail))
                {
                    var result = await ShowMessageBoxAsync("Data Updated!",
                        "Data loaded successfully. Click Ok if you want to log in cashier.");

                    if (result == ButtonResult.Ok)
                    {
                        new LogInWindow().Show();
                    }
                }
                else
                {
                    new LogInWindow().Show();
                }
            }
            finally
            {
                ShowLoader(false);
                Close();
            }
        }
        private async Task<ButtonResult> ShowMessageBoxAsync(string header, string message)
        {
            return await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentHeader = header,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.Ok,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                Width = 400,
                ShowInCenter = true
            }).ShowWindowDialogAsync(this);
        }
        private async void Refund_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var refundOrder = new SetCashDrawerWindow("Returned");
            await refundOrder.ShowDialog(this);
        }

        private async void LogOut_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (OrderState.CurrentOrder.Any())
            {

                await MessageBoxManager.GetMessageBoxStandard(
                    new MessageBoxStandardParams
                    {
                        ContentHeader = $"Error",
                        ContentMessage = "Unable to log out – there is a pending order.",
                        ButtonDefinitions = ButtonEnum.Ok, // Defines the available buttons
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        CanResize = false,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        Width = 400,
                        ShowInCenter = true,
                        Icon = MsBox.Avalonia.Enums.Icon.Error
                    }).ShowAsPopupAsync(this);
                return;
            }

            var box = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentHeader = $"Log Out",
                    ContentMessage = "Please ask the manager to swipe.",
                    ButtonDefinitions = ButtonEnum.OkCancel, // Defines the available buttons
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Width = 400,
                    ShowInCenter = true,
                    Icon = MsBox.Avalonia.Enums.Icon.Warning
                });

            var managerEmail = "user1@example.com";

            var result = await box.ShowAsPopupAsync(this);
            switch (result)
            {
                case ButtonResult.Ok:

                    ShowLoader(true);
                    var setCashDrawer = new SetCashDrawerWindow("Cash-Out");
                    await setCashDrawer.ShowDialog(this);

                    ReceiptPrinterUtil.PrintXReading(_serviceProvider!);

                    // Open the TenderOrderWindow
                    var (isSuccess, Message) = await _authService.LogOut(managerEmail);
                    if (isSuccess)
                    {
                        CashierState.CashierName = null;
                        CashierState.CashierEmail = null;
                        OrderState.CurrentOrder.Clear();
                        OrderState.CurrentOrderItem = new OrderItemState();
                        TenderState.tenderOrder.Reset();

                        var logInWindow = new LogInWindow();
                        logInWindow.Show();
                        ShowLoader(false);
                        Close();
                    }
                    return;
                case ButtonResult.Cancel:
                    return;
                default:
                    return;
            }

        }

    }
}