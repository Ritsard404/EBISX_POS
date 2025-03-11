using System.Collections.ObjectModel;
using EBISX_POS.ViewModels; // Ensure this is added
using EBISX_POS.API.Models; // Ensure this is added
using EBISX_POS.State;
using System.Threading.Tasks;
using EBISX_POS.Services;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EBISX_POS.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly MenuService _menuService;


        public ObservableCollection<Category> ButtonList { get; } = new();
        public OrderSummaryViewModel OrderSummaryViewModel { get; } // Add this property
        public ItemListViewModel ItemListViewModel { get; } // Add this property

        public MainWindowViewModel(MenuService menuService)
        {
            _menuService = menuService;
            OrderSummaryViewModel = new OrderSummaryViewModel(); // Initialize it
            ItemListViewModel = new ItemListViewModel(menuService); // Initialize it

            _ = LoadCategories();



            // Subscribe to changes in the entire OrderState
            OrderState.StaticPropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(OrderState.CurrentOrderItem))
                {
                    OnPropertyChanged(nameof(CashierName));
                }
            };

            // Also subscribe to Quantity changes (Nested Property)
            if (OrderState.CurrentOrderItem != null)
            {
                OrderState.CurrentOrderItem.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(OrderState.CurrentOrderItem.Quantity))
                    {
                        OnPropertyChanged(nameof(CashierName));
                    }
                };
            }
        }

        public string CashierName => OrderState.CurrentOrderItem.Quantity.ToString();
        //public string CashierName => CashierState.CashierName ?? "Developer";

        private async Task LoadCategories()
        {
            var categories = await _menuService.GetCategoriesAsync();
            ButtonList.Clear();
            categories.ForEach(category => ButtonList.Add(category));

            await LoadMenusAsync(categories.FirstOrDefault().Id);

        }

        public async Task LoadMenusAsync(int categoryId)
        {
            Debug.WriteLine($"Loading menus for category ID: {categoryId}");
            await ItemListViewModel.LoadMenusAsync(categoryId);
            Debug.WriteLine($"Finished loading menus for category ID: {categoryId}");
        }
    }
}
