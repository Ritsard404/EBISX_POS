using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Microsoft.Extensions.DependencyInjection;
using EBISX_POS.Services;
using Avalonia.Controls.ApplicationLifetimes;
using EBISX_POS.ViewModels;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using EBISX_POS.API.Services.DTO.Auth;
using EBISX_POS.State;
using EBISX_POS.Models;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Extensions.Options;

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
            _menuService = _serviceProvider.GetRequiredService<MenuService>();
            _authService = _serviceProvider.GetRequiredService<AuthService>();


            var reportOptions = _serviceProvider.GetRequiredService<IOptions<SalesReport>>();
            _cashTrackReportPath = reportOptions.Value.CashTrackReport;


            _cashTrackReportPath = reportOptions.Value.CashTrackReport;
        }

        public ManagerWindow() : this(App.Current.Services.GetRequiredService<IServiceProvider>())
        {
            // This constructor is required for Avalonia to instantiate the view in XAML.
        }

        private void SummaryReport_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var reportWindow = _serviceProvider?.GetRequiredService<DailySalesReportView>();
            reportWindow?.Show();
        }

        private void TransactionLog(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var transactionWindow = _serviceProvider.GetRequiredService<TransactionView>();
            transactionWindow.Show();
        }

        private async void Cash_Track_Button(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //var cashTrack = _serviceProvider?.GetRequiredService<CashTrackView>();
            //if (cashTrack != null)
            //{
            //    cashTrack.GenerateCashTrack(sender, e);
            //}


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
        }

        private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_menuService, _authService)
            {
                DataContext = new MainWindowViewModel(_menuService)
            };
            mainWindow.Show();

            Close();

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
                        Close();
                    }
                    Debug.WriteLineIf(!isSuccess, Message);
                    return;
                case ButtonResult.Cancel:
                    return;
                default:
                    return;
            }

        }
    }
}