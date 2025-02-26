using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EBISX_POS.Views
{
    public partial class SubItemWindow : Window
    {
        public SubItemWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
};
