using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.API.Services.DTO.Auth;
using EBISX_POS.Services;
using EBISX_POS.State;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels
{
    public partial class LogInWindowViewModel : ViewModelBase
    {

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage; 
        
        [ObservableProperty]
        private CashierDTO? _selectedCashier;

        [ObservableProperty]
        private string _managerEmail;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        private readonly AuthService _authService;
        public ObservableCollection<CashierDTO> Cashiers { get; } = new();

        private void SetProperty(ref bool isLoading, bool value)
        {
            throw new NotImplementedException();
        }

        public LogInWindowViewModel(AuthService authService)
        {
            _authService = authService;
            LoadCashiers();
        }

        public async Task LoadCashiers()
        {
            try
            {
                IsLoading = true;
                var cashiers = await _authService.GetCashiersAsync();
                Cashiers.Clear();
                cashiers.ForEach(cashier => Cashiers.Add(cashier));
                Debug.WriteLine($"Loaded {Cashiers.Count} cashiers.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}"); 
                NotificationService.NetworkIssueMessage();

            }
            finally
            {
                IsLoading = false;
            }
        }

        // #TODO: Implement Transition to POS Dashboard
        [RelayCommand]
        private async Task LogIn()
        {
            try
            {
                ErrorMessage = "";
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

                // Set Cashier Name in Global State
                CashierState.CashierName = message;
                Debug.WriteLine($"Log in success: {message}"); // Debug line
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                ErrorMessage = "An unexpected error occurred.";
                OnPropertyChanged(nameof(HasError)); 
                NotificationService.NetworkIssueMessage();

            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
