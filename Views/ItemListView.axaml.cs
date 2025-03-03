using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using EBISX_POS.ViewModels;
using EBISX_POS.Services; // Ensure this is added
using System.Diagnostics;
using System.Threading.Tasks;

namespace EBISX_POS.Views
{
    public partial class ItemListView : UserControl
    {
        private ToggleButton? _selectedItemButton;
        private string? _selectedItem;

        public ItemListView(MenuService menuService) // Ensure this constructor is public
        {
            InitializeComponent();
            DataContext = new ItemListViewModel(menuService); // Set initial DataContext
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

                var detailsWindow = new SubItemWindow
                {
                    DataContext = new SubItemWindowViewModel(item)
                };

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
