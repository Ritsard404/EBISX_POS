using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using EBISX_POS.API.Models;
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace EBISX_POS.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly MenuService _menuService;
        private ToggleButton? _selectedMenuButton; // Stores selected menu item
        private Category? _selectedMenuItem;       // Stores selected menu item object

        private bool isLoading = true;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow(MenuService menuService)
        {
            InitializeComponent();
            _menuService = menuService;
            DataContext = new MainWindowViewModel(menuService);

            // Create and set the ItemListView
            var itemListView = CreateItemListView();
            ItemListViewContainer.Content = itemListView;

            // When the window is opened, load the first category's menus.
            this.Opened += async (s, e) =>
            {
                var categories = await _menuService.GetCategoriesAsync();
                if (categories.Any())
                {
                    var firstCategory = categories.First();
                    
                    // Set loading flag to true
                    IsLoadCtgry.IsVisible = true;
                    IsLoadMenu.IsVisible = true;
                    IsCtgryAvail.IsVisible = false;
                    IsMenuAvail.IsVisible = false;

                    await itemListView.LoadMenusAsync(firstCategory.Id);
                    
                    // Once loaded, set loading flag to false.
                    IsLoadCtgry.IsVisible = false;
                    IsLoadMenu.IsVisible = false;
                    IsCtgryAvail.IsVisible = true;
                    IsMenuAvail.IsVisible = true;
                }
            };

            // Attach to the ItemsControl's AttachedToVisualTree to auto-select the first toggle.
            MenuGroup.AttachedToVisualTree += (s, e) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (MenuGroup.ItemContainerGenerator.ContainerFromIndex(0) is ToggleButton firstToggle)
                    {
                        firstToggle.IsChecked = true;
                        ToggleButton_Click(firstToggle, new RoutedEventArgs());
                    }
                }, DispatcherPriority.Background);
            };

             
        }

        private ItemListView CreateItemListView()
        {
            return new ItemListView(_menuService);
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton clickedButton)
            {
                // Get the Category object from the clicked button's DataContext.
                var menuItem = clickedButton.DataContext as Category;

                // Deselect previous button if it's different.
                if (_selectedMenuButton != null && _selectedMenuButton != clickedButton)
                {
                    _selectedMenuButton.IsChecked = false;
                }

                // Toggle the current button's state.
                bool isSelected = clickedButton.IsChecked ?? false;
                _selectedMenuButton = isSelected ? clickedButton : null;
                _selectedMenuItem = isSelected ? menuItem : null;

                if (_selectedMenuItem != null)
                {
                    if (DataContext is MainWindowViewModel viewModel)
                    {
                        Debug.WriteLine($"Calling LoadMenusAsync for category ID: {_selectedMenuItem.Id}");
                        // Set loading flag to true before loading
                        IsLoading = true;
                        await viewModel.LoadMenusAsync(_selectedMenuItem.Id);
                        Debug.WriteLine($"Finished calling LoadMenusAsync for category ID: {_selectedMenuItem.Id}");

                        // Update ItemListView's DataContext.
                        if (ItemListViewContainer.Content is ItemListView itemListView)
                        {
                            await itemListView.LoadMenusAsync(_selectedMenuItem.Id);
                        }
                        // Set loading flag to false after loading
                        IsLoading = false;
                    }
                }
            }
        }
    }
}
