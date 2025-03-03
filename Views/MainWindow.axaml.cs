using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using EBISX_POS.API.Models; // Ensure this is added
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using System.Diagnostics;

namespace EBISX_POS.Views
{
    public partial class MainWindow : Window
    {
        private readonly CashierState _cashierState;
        private readonly MenuService _menuService;

        private ToggleButton? _selectedMenuButton; // Stores selected menu item
        private Category? _selectedMenuItem;       // Stores selected menu item object
        public MainWindow(CashierState cashierState, MenuService menuService)
        {
            InitializeComponent();
            _cashierState = cashierState;
            _menuService = menuService;
            DataContext = new MainWindowViewModel(cashierState, menuService);

            // Create and set the ItemListView
            var itemListView = CreateItemListView();
            ItemListViewContainer.Content = itemListView;
        }

        private ItemListView CreateItemListView()
        {
            return new ItemListView(_menuService);
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton clickedButton)
            {
                // Get the Category object of the clicked button
                var menuItem = clickedButton.DataContext as Category;

                // Deselect previous button
                if (_selectedMenuButton != null && _selectedMenuButton != clickedButton)
                {
                    _selectedMenuButton.IsChecked = false;
                }

                // Toggle the current button state
                bool isSelected = clickedButton.IsChecked ?? false;
                _selectedMenuButton = isSelected ? clickedButton : null;
                _selectedMenuItem = isSelected ? menuItem : null;

                if (_selectedMenuItem != null)
                {
                    var viewModel = DataContext as MainWindowViewModel;
                    if (viewModel != null)
                    {
                        Debug.WriteLine($"Calling LoadMenusAsync for category ID: {_selectedMenuItem.Id}");
                        await viewModel.LoadMenusAsync(_selectedMenuItem.Id);
                        Debug.WriteLine($"Finished calling LoadMenusAsync for category ID: {_selectedMenuItem.Id}");

                        // Update ItemListView's DataContext
                        var itemListView = ItemListViewContainer.Content as ItemListView;
                        if (itemListView != null)
                        {
                            await itemListView.LoadMenusAsync(_selectedMenuItem.Id);
                        }
                    }
                }
            }
        }
    }
}