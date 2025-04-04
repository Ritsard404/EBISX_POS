using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace EBISX_POS.Views
{
    public partial class ManagerSwipeWindow : Window
    {
        private readonly TaskCompletionSource<bool> _completionSource = new();

        public ManagerSwipeWindow(string header, string message, string ButtonName)
        {
            InitializeComponent();
            DataContext = this;

            // Find controls
            HeaderTextBlock = this.FindControl<TextBlock>("HeaderTextBlock");
            BodyMessageTextBlock = this.FindControl<TextBlock>("BodyMessageTextBlock");
            SwipeButton = this.FindControl<Button>("SwipeButton");

            // Debugging - Check if controls are found
            if (HeaderTextBlock == null)
            {
                Console.WriteLine("? HeaderTextBlock is NULL! Check XAML x:Name.");
            }
            else
            {
                HeaderTextBlock.Text = header;
            }

            if (BodyMessageTextBlock == null)
            {
                Console.WriteLine("? BodyMessageTextBlock is NULL! Check XAML x:Name.");
            }
            else
            {
                BodyMessageTextBlock.Text = message;
            }

            if (SwipeButton == null)
            {
                Console.WriteLine("? BodyMessageTextBlock is NULL! Check XAML x:Name.");
            }
            else
            {
                SwipeButton.Content = ButtonName;
            }


            // Close the dialog automatically after 5 seconds if no action is taken
            DispatcherTimer.RunOnce(() =>
            {
                if (!_completionSource.Task.IsCompleted)
                {
                    _completionSource.SetResult(false);
                    Close(false);
                }
            }, TimeSpan.FromSeconds(5));
        }

        public Task<bool> ShowDialogAsync(Window parent)
        {
            ShowDialog(parent);
            return _completionSource.Task;
        }

        private void OnSwipeClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _completionSource.SetResult(true);
            Close(true);
        }
    }
}
