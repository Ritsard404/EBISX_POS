using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.Services;
using EBISX_POS.Services.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels
{
    public class LogInWindowViewModel : ViewModelBase
    {
        private bool _isLoading;
        private readonly AuthService _authService;
        public ObservableCollection<CashierDTO> Cashiers { get; } = new();
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public LogInWindowViewModel(AuthService authService)
        {
            _authService = authService;
            LoadCashiers();
        }

        public async void LoadCashiers()
        {
            try
            {
                IsLoading = true;
                var cashiers = await _authService.GetCashiersAsync();
                Cashiers.Clear();
                cashiers.ForEach(cashier => Cashiers.Add(cashier));
                Debug.WriteLine($"Loaded {Cashiers.Count} cashiers."); // Debug line
            }
            catch (Exception ex)
            {
                IsLoading = true;
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
