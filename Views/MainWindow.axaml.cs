using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using MsBox.Avalonia.Enums;
using EBISX_POS.API.Models;
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia;
using System.ComponentModel;
using System.Linq;
using EBISX_POS.Models;
using System;
using System.Diagnostics;

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



            this.AttachedToVisualTree += OnAttachedToVisualTree;

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

            OnPropertyChanged(nameof(OrderState.CurrentOrderItem));

        }


        private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            // Get all ToggleButtons that are part of the ItemsControl's visual tree.
            var toggleButtons = MenuGroup.GetLogicalDescendants().OfType<ToggleButton>().ToList();

            if (toggleButtons.Any())
            {
                // Set the first ToggleButton as checked.
                _selectedMenuButton = toggleButtons.First();
                _selectedMenuButton.IsChecked = true;
            }
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
                        // Set loading flag to true before loading
                        IsLoading = true;
                        await viewModel.LoadMenusAsync(_selectedMenuItem.Id);

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
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Content?.ToString(), out int digit))
            {
                if (!OrderState.CurrentOrderItem.SubOrders.Any())
                {
                    OrderState.CurrentOrderItem.Quantity = digit;
                    OrderState.UpdateItemOrder(itemType: "Menu", itemId: digit, name: "Select Menu", price: 0, size: null);
                    OrderState.CurrentOrderItem.RefreshDisplaySubOrders();
                    return;
                }

                // Build new quantity string and parse it back to int
                string newString = $"{(OrderState.CurrentOrderItem.Quantity == 0 ? "" : OrderState.CurrentOrderItem.Quantity)}{digit}";

                if (int.TryParse(newString, out int newValue))
                {
                    OrderState.CurrentOrderItem.Quantity = newValue;
                    OnPropertyChanged(nameof(OrderState.CurrentOrderItem));
                }

            }
        }

        private void ClearNumber_Click(object sender, RoutedEventArgs e)
        {
            OrderState.CurrentOrderItem.Quantity = 0;
        }

        private async void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!OrderState.CurrentOrder.Any())
                return;

            var box = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentHeader = "Cancel Order",
                    ContentMessage = "Swipe the manager ID.",
                    ButtonDefinitions = ButtonEnum.OkCancel, // Defines the available buttons
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Width = 400,
                    ShowInCenter = true,
                    Icon = MsBox.Avalonia.Enums.Icon.Warning
                });

            var result = await box.ShowAsPopupAsync(this);

            switch (result)
            {
                case ButtonResult.Ok:
                    OrderState.CurrentOrderItem = new OrderItemState();
                    OrderState.CurrentOrder.Clear();
                    OrderState.CurrentOrderItem.RefreshDisplaySubOrders();
                    return;
                case ButtonResult.Cancel:
                    return;
                default:
                    return;
            }


        }

        private async void OrderType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string orderType = string.Empty;

                if (btn.Content is TextBlock textBlock)
                {
                    orderType = textBlock.Text;
                }
                else
                {
                    orderType = btn.Content?.ToString() ?? string.Empty;
                }

                // Perform actions based on the orderType
                switch (orderType)
                {
                    case "DINE IN":
                        // Handle Dine In logic
                        OrderState.CurrentOrderItem.OrderType = "Dine In";
                        break;
                    case "TAKE OUT":
                        // Handle Take Out logic
                        OrderState.CurrentOrderItem.OrderType = "Take Out";
                        break;
                    default:
                        // Handle other cases if necessary
                        break;
                }

                TenderState.tenderOrder.Reset();
                if (TenderState.tenderOrder.CalculateTotalAmount())
                {

                    var box = MessageBoxManager.GetMessageBoxStandard(
                        new MessageBoxStandardParams
                        {
                            ContentHeader = "No Order Yet!",
                            ContentMessage = "Please Select Order.",
                            ButtonDefinitions = ButtonEnum.Ok, // Defines the available buttons
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            CanResize = false,
                            SizeToContent = SizeToContent.WidthAndHeight,
                            Width = 400,
                            ShowInCenter = true,
                            Icon = MsBox.Avalonia.Enums.Icon.Warning
                        });

                    var result = await box.ShowAsPopupAsync(this);

                    switch (result)
                    {
                        case ButtonResult.Ok:
                            return;
                        default:
                            return;
                    }
                }

                // Open the TenderOrderWindow
                var tenderOrderWindow = new TenderOrderWindow();
                await tenderOrderWindow.ShowDialog((Window)this.VisualRoot);
            }
        }
    }
}
