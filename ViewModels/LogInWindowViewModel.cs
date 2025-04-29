using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.API.Services.DTO.Auth;
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.Views;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;

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
            _ = CheckData();
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

        private async Task CheckData()
        {
            try
            {
                IsLoading = true;
                var (isSuccess, message) = await _authService.CheckData();
                if (isSuccess)
                {
                    var owner = GetCurrentWindow();
                    if (owner == null)
                        return;

                    var managerWindow = new ManagerWindow();

                    if (Application.Current.ApplicationLifetime
                        is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = managerWindow;
                    }

                    managerWindow.Show();
                    owner.Close();

                }
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
                var (success, cashierName, cashierEmail) = await _authService.HasPendingOrder();
                if (success)
                {
                    var owner = GetCurrentWindow();

                    var alertBox = MessageBoxManager.GetMessageBoxStandard(
                        new MessageBoxStandardParams
                        {
                            ContentHeader = "Cashier Session Restored",
                            ContentMessage = "Your previous session has been successfully restored after an unexpected closure. Please review your cashier data and continue. If you experience further issues, verify your network connection.",
                            ButtonDefinitions = ButtonEnum.Ok,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            CanResize = false,
                            SizeToContent = SizeToContent.WidthAndHeight,
                            Width = 400,
                            ShowInCenter = true
                        });

                    await alertBox.ShowWindowDialogAsync(owner);

                    NavigateToMainWindow(cashierEmail: cashierEmail, cashierName: cashierName);
                    owner.Close();
                }
                IsLoading = false;
                return;
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
                CashierState.CashierStateReset();

                var owner = GetCurrentWindow();
                ErrorMessage = string.Empty;
                IsLoading = true;

                //if (SelectedCashier == null)
                //{
                //    ErrorMessage = "Please select a cashier.";
                //    OnPropertyChanged(nameof(HasError));
                //    return;
                //}

                var logInDTO = new LogInDTO
                {
                    CashierEmail = SelectedCashier?.Email ?? "",
                    ManagerEmail = ManagerEmail
                };

                var (success, isManager, name, email) = await _authService.LogInAsync(logInDTO);
                if (!success)
                {
                    ErrorMessage = name;
                    OnPropertyChanged(nameof(HasError));
                    return;
                }

                if (isManager)
                {
                    CashierState.ManagerEmail = email;
                    owner.Close();
                    var managerWindow = new ManagerWindow();
                    if (Application.Current.ApplicationLifetime
                        is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = managerWindow;
                    }
                    managerWindow.Show();
                    return;
                }

                NavigateToMainWindow(cashierEmail: email, cashierName: name);
                owner.Close();
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
        private Window? GetCurrentWindow()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }

        private void NavigateToMainWindow(string cashierEmail, string cashierName)
        {
            CashierState.CashierEmail = cashierEmail;
            CashierState.CashierName = cashierName;

            var mainWindow = new MainWindow(_menuService, _authService)
            {
                DataContext = new MainWindowViewModel(_menuService)
            };
            mainWindow.Show();
        }
    }
}
