using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.Services;
using EBISX_POS.ViewModels;

namespace EBISX_POS.Views {

    public partial class CrewLogEntryView : Window
    {
        private readonly CrewLogEntryViewModel _viewModel;

        public CrewLogEntryView()
        {
            InitializeComponent();
            // Create a CrewServices instance and pass it to the ViewModel
            var crewService = new CrewServices();
            DataContext = new CrewLogEntryViewModel(crewService);

        }
    }

};