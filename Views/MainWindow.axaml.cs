using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using EBISX_POS.ViewModels;
using System.Diagnostics;

namespace EBISX_POS.Views
{
    public partial class MainWindow : Window
    {
        private ToggleButton? _selectedMenuButton; // Stores selected menu item
        private string? _selectedMenuText;        // Stores selected menu item text
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton clickedButton)
            {
                // Get the text of the clicked button
                string? buttonText = (clickedButton.Content as TextBlock)?.Text;

                // Deselect previous button
                if (_selectedMenuButton != null && _selectedMenuButton != clickedButton)
                {
                    _selectedMenuButton.IsChecked = false;
                }

                // Toggle the current button state
                bool isSelected = clickedButton.IsChecked ?? false;
                _selectedMenuButton = isSelected ? clickedButton : null;
                _selectedMenuText = isSelected ? buttonText : null;

                Debug.WriteLine($"Selected Menu Item: {_selectedMenuText}");
            }
        }
    }
}