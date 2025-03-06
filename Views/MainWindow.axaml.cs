using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using EBISX_POS.API.Models; // Ensure this is added
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using System.Diagnostics;

namespace EBISX_POS.Views
{
    public partial class MainWindow : Window
    {
        private readonly MenuService _menuService;
        private ToggleButton? _selectedMenuButton; // Stores selected menu item
        private Category? _selectedMenuItem;       // Stores selected menu item object

        public MainWindow(MenuService menuService)
        {
            InitializeComponent();
            _menuService = menuService;
            DataContext = new MainWindowViewModel(menuService);

            // Create and set the ItemListView
            var itemListView = CreateItemListView();
            ItemListViewContainer.Content = itemListView;

            // Subscribe to the ItemsControl's attached event.
            // Assume MenuGroup is the x:Name of your ItemsControl that hosts the ToggleButtons.
            MenuGroup.AttachedToVisualTree += MenuGroup_AttachedToVisualTree;
        }

        private ItemListView CreateItemListView()
        {
            return new ItemListView(_menuService);
        }

        /// <summary>
        /// When the ItemsControl is attached to the visual tree, this method is called.
        /// It retrieves the first ToggleButton (the default category) and toggles it.
        /// </summary>
        private async void MenuGroup_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            // Post the action to the UI thread to ensure all items are generated.
            Dispatcher.UIThread.Post(async () =>
            {
                // Get the container for the first item in the ItemsControl.
                var container = MenuGroup.ItemContainerGenerator.ContainerFromIndex(0) as Control;
                if (container is ToggleButton firstToggleButton)
                {
                    // Set the first button as checked.
                    firstToggleButton.IsChecked = true;
                    // Call the click handler to load menus for the default category.
                    ToggleButton_Click(firstToggleButton, new RoutedEventArgs());
                }
            }, DispatcherPriority.Background);
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
                        await viewModel.LoadMenusAsync(_selectedMenuItem.Id);
                        Debug.WriteLine($"Finished calling LoadMenusAsync for category ID: {_selectedMenuItem.Id}");

                        // Update ItemListView's DataContext.
                        if (ItemListViewContainer.Content is ItemListView itemListView)
                        {
                            await itemListView.LoadMenusAsync(_selectedMenuItem.Id);
                        }
                    }
                }
            }
        }
    }
}
