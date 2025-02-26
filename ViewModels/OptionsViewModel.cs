using Avalonia.Controls;
using EBISX_POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels
{
    public class OptionsViewModel : ViewModelBase
    {
        public ObservableCollection<ItemMenu> OptionItems { get; set; }

        public OptionsViewModel()
        {
            // Sample data
            OptionItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu { Id = 1, ItemName = "Coke", Price = 0.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/coke.jpg" },
                new ItemMenu { Id = 2, ItemName = "Coke Zero", Price = 1.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/zero.jpg" },
                new ItemMenu { Id = 3, ItemName = "Ice Tea", Price = .59m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/tea.jpg" },
                new ItemMenu { Id = 4, ItemName = "Sprite", Price = 9m, ImagePath  = "avares://EBISX_POS/Assets/Images/Drinks/sprite.jpg" }
            };
        }
    }
}
