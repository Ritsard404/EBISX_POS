using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using EBISX_POS.Models;
using EBISX_POS.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace EBISX_POS.Views
{
    public partial class OptionsView : UserControl
    {
        private ToggleButton? _selectedOptionButton; // Stores selected option (Cold Drinks / Hot Drinks)
        private ToggleButton? _selectedItemButton;   // Stores selected menu item
        private ToggleButton? _selectedSizeButton;   // Stores selected size (Regular / Medium / Large)

        private string? _selectedOption;  // Store selected option text
        private string? _selectedItem;    // Store selected menu item text
        private string? _selectedSize;    // Store selected size text

        public OptionsView()
        {
            InitializeComponent();
            DataContext = new SubItemWindowViewModel();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton clickedButton)
            {
                var parentStackPanel = clickedButton.Parent as StackPanel;

                // Determine which group this button belongs to
                if (parentStackPanel != null)
                {
                    if (parentStackPanel.Name == "OptionsGroup")
                    {
                        HandleSelection(ref _selectedOptionButton, clickedButton, ref _selectedOption);
                        Debug.WriteLine($"Selected Option: {_selectedOption}");
                    }
                    else if (parentStackPanel.Name == "SizeGroup")
                    {
                        HandleSelection(ref _selectedSizeButton, clickedButton, ref _selectedSize);
                        Debug.WriteLine($"Selected Size: {_selectedSize}");
                    }
                }
                else if (clickedButton.DataContext is ItemMenu item)
                {
                    HandleSelection(ref _selectedItemButton, clickedButton, ref _selectedItem);
                    Debug.WriteLine($"Selected Item: {item.ItemName}");
                }
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
