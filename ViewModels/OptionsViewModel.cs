using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using System.Collections.ObjectModel;

namespace EBISX_POS.ViewModels
{
    public partial class OptionsViewModel : ViewModelBase
    {
        // Observable Collection for Menu Items
        public ObservableCollection<ItemMenu> OptionItems { get; set; }

        public OptionsViewModel()
        {
            // Sample data
            OptionItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu { Id = 1, ItemName = "Coke", Price = 0.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/coke.jpg" },
                new ItemMenu { Id = 2, ItemName = "Coke Zero", Price = 1.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/zero.jpg" },
                new ItemMenu { Id = 3, ItemName = "Ice Tea", Price = 0.59m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/tea.jpg" },
                new ItemMenu { Id = 4, ItemName = "Sprite", Price = 9m, ImagePath  = "avares://EBISX_POS/Assets/Images/Drinks/sprite.jpg" }
            };
        }
    }
}
