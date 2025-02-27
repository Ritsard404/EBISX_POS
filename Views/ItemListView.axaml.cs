using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using EBISX_POS.ViewModels;
using System.Diagnostics;

namespace EBISX_POS.Views
{
    public partial class ItemListView : UserControl
    {
        private ToggleButton? _selectedItemButton;
        private string? _selectedItem;

        public ItemListView()
        {
            InitializeComponent();
            DataContext = new ItemListViewModel();
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
