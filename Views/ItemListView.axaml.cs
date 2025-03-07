using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity; // Add this line
using EBISX_POS.Models;
using EBISX_POS.ViewModels;
using EBISX_POS.Services; // Ensure this is added
using System.Diagnostics;
using System.Threading.Tasks;
using EBISX_POS.State;
using EBISX_POS.API.Models;
using System.Linq;

namespace EBISX_POS.Views
{
    public partial class ItemListView : UserControl
    {
        private readonly MenuService _menuService;


        private ToggleButton? _selectedItemButton;
        private string? _selectedItem;

        public ItemListView(MenuService menuService) // Ensure this constructor is public
        {
            InitializeComponent();
            _menuService = menuService;
            DataContext = new ItemListViewModel(menuService); // Set initial DataContext
            this.Loaded += OnLoaded; // Add this line
        }

        private void OnLoaded(object? sender, RoutedEventArgs e) // Update the event handler signature
        {
            if (ItemsList.ItemContainerGenerator.ContainerFromIndex(0) is ToggleButton firstButton)
            {
                firstButton.IsChecked = true;
                _selectedItemButton = firstButton;
                _selectedItem = firstButton.Content?.ToString();
            }
        }

        public async Task LoadMenusAsync(int categoryId)
        {
            if (DataContext is ItemListViewModel viewModel)
            {
                await viewModel.LoadMenusAsync(categoryId);
            }
        }

        public void UpdateDataContext(ItemListViewModel viewModel)
        {
            DataContext = viewModel;
        }

        private async void OnItemClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is ToggleButton clickedButton && clickedButton.DataContext is ItemMenu item)
            {
                HandleSelection(ref _selectedItemButton, clickedButton, ref _selectedItem);
                Debug.WriteLine($"Selected Item: {item.ItemName}");

                if (item.IsSolo || item.IsAddOn)
                    return;

                var detailsWindow = new SubItemWindow(item, _menuService)
                {
                    DataContext = new SubItemWindowViewModel(item, _menuService)
                };

                OrderState.CurrentOrderItem.SubOrders.Clear();
                OrderState.CurrentOrderItem.SubOrders.Add(new SubOrderItem
                {
                    MenuId = item.Id,
                    Name = item.ItemName,
                    ItemPrice = item.Price
                });

                await detailsWindow.ShowDialog((Window)this.VisualRoot);
            }
        }

        private void HandleSelection(ref ToggleButton? selectedButton, ToggleButton clickedButton, ref string? selectedValue)
        {
            if (selectedButton == clickedButton)
            {
                clickedButton.IsChecked = false;
                selectedButton = null;
                selectedValue = null;
            }
            else
            {
                if (selectedButton != null)
                {
                    selectedButton.IsChecked = false;
                }

                clickedButton.IsChecked = true;
                selectedButton = clickedButton;
                selectedValue = clickedButton.Content?.ToString();
            }
        }
    }
}
