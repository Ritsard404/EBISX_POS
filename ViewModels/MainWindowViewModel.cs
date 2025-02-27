using System.Collections.ObjectModel;
using EBISX_POS.ViewModels; // Ensure this is added


namespace EBISX_POS.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        public ObservableCollection<string> ButtonList { get; } = new ObservableCollection<string>
        {
            "Regular Menu",
            "Breakfast Menu",
            "Desserts Menu",
            "Drinks Menu",
            "Mix & Match Menu",
            "Add Ons",
            "Combo Meals",
        };
        public OrderSummaryViewModel OrderSummaryViewModel { get; } // Add this property

        public MainWindowViewModel()
        {
            OrderSummaryViewModel = new OrderSummaryViewModel(); // Initialize it
        }
    }
}
