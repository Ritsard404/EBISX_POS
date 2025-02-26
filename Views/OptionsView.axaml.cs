using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using EBISX_POS.ViewModels;
using System.Diagnostics;

namespace EBISX_POS.Views
{
    public partial class OptionsView : UserControl
    {
        public string SelectedSize { get; private set; } = string.Empty; // Store selected button content

        public OptionsView()
        {
            InitializeComponent();
            DataContext = new OptionsViewModel();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton clickedButton)
            {
                // Store the selected button's content (e.g., "Small", "Medium", "Large")
                SelectedSize = clickedButton.Content.ToString();

                foreach (var child in (clickedButton.Parent as StackPanel).Children)
                {
                    if (child is ToggleButton button && button != clickedButton)
                    {
                        button.IsChecked = false;
                    }
                }

                // Debugging (Optional)
                Debug.WriteLine($"Selected Size: {SelectedSize}");
            }
        }

        private void ToggleButton_IsCheckedChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is ToggleButton clickedButton && clickedButton.IsChecked == true)
            {
                foreach (var child in (clickedButton.Parent as StackPanel).Children)
                {
                    if (child is ToggleButton button && button != clickedButton)
                    {
                        button.IsChecked = false;
                    }
                }
            }
        }
    }
};
