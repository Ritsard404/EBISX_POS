using System.Collections.ObjectModel;

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
    }
}
