using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using EBISX_POS.ViewModels;
using System.Diagnostics;

namespace EBISX_POS.Views
{
    public partial class ItemListView : UserControl
    {
        public ItemListView()
        {
            InitializeComponent();
            DataContext = new ItemListViewModel();
        }

        private async void OnItemClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ItemMenu item)
            {
                var detailsWindow = new SubItemWindow
                {
                    DataContext = new SubItemWindowViewModel(item)
                };

                await detailsWindow.ShowDialog((Window)this.VisualRoot);
            }
        }
    }
}
