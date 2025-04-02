using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.API.Services.DTO.Auth;
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels
{
    public partial class LogInWindowViewModel : ViewModelBase
    {
        private readonly MenuService _menuService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private CashierDTO? selectedCashier;

        [ObservableProperty]
        private string managerEmail;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ObservableCollection<CashierDTO> Cashiers { get; } = new();

        public LogInWindowViewModel(AuthService authService, MenuService menuService)
        {
            _authService = authService;
            _menuService = menuService;

            // Fire-and-forget async calls
            _ = LoadCashiersAsync();
            _ = CheckPendingOrderAsync();
        }

        private async Task LoadCashiersAsync()
        {
            try
            {
                IsLoading = true;
                var cashiers = await _authService.GetCashiersAsync();
                Cashiers.Clear();
                foreach (var cashier in cashiers)
                {
                    Cashiers.Add(cashier);
                }
                Debug.WriteLine($"Loaded {Cashiers.Count} cashiers.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading cashiers: {ex.Message}");
                NotificationService.NetworkIssueMessage();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CheckPendingOrderAsync()
        {

            try
            {
                IsLoading = true;
                var (success, message) = await _authService.HasPendingOrder();
                if (success)
                {
                    NavigateToMainWindow(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking pending order: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LogInAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                IsLoading = true;

                if (SelectedCashier == null)
                {
                    ErrorMessage = "Please select a cashier.";
                    OnPropertyChanged(nameof(HasError));
                    return;
                }

                var logInDTO = new LogInDTO
                {
                    CashierEmail = SelectedCashier.Email,
                    ManagerEmail = ManagerEmail
                };

                var (success, message) = await _authService.LogInAsync(logInDTO);
                if (!success)
                {
                    ErrorMessage = message;
                    OnPropertyChanged(nameof(HasError));
                    return;
                }

                NavigateToMainWindow(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Log in error: {ex.Message}");
                ErrorMessage = "An unexpected error occurred.";
                OnPropertyChanged(nameof(HasError));
                NotificationService.NetworkIssueMessage();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NavigateToMainWindow(string cashierName)
        {
            CashierState.CashierName = cashierName;
            var mainWindow = new MainWindow(_menuService)
            {
                DataContext = new MainWindowViewModel(_menuService)
            };
            mainWindow.Show();

            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var loginWindow = desktopLifetime.MainWindow as LogInWindow;
                loginWindow?.Close();  // Close the login window
            }
        }
    }
}
