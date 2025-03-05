using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.ViewModels;
using EBISX_POS.Models;
using EBISX_POS.Services;
using EBISX_POS.State;

namespace EBISX_POS.Views
{
    public partial class SubItemWindow : Window
    {
        public SubItemWindow(ItemMenu item, MenuService menuService)
        {
            InitializeComponent();
            DataContext = new SubItemWindowViewModel(item, menuService);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
