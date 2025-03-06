using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using EBISX_POS.Services.DTO.Menu;
using EBISX_POS.State;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels
{
    public partial class OptionsViewModel : ViewModelBase
    {
        public ObservableCollection<ItemMenu> OptionItems { get; set; }
        public bool HasDrinks => OptionsState.DrinkTypes.Any();
        public bool HasAddOns => OptionsState.AddOnsType.Any();


        public ObservableCollection<DrinkTypeDTO> DrinkTypes { get; } = OptionsState.DrinkTypes;
        public ObservableCollection<DrinkDetailDTO> Drinks { get; } = OptionsState.Drinks;
        public ObservableCollection<string> DrinkSizes { get; } = OptionsState.DrinkSizes;
        public ObservableCollection<AddOnTypeDTO> AddOnsType { get; } = OptionsState.AddOnsType;
        public ObservableCollection<AddOnDetailDTO> AddOns { get; } = OptionsState.AddOns;

        public OptionsViewModel()
        {
            OptionItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu { Id = 1, ItemName = "Coke", Price = 0.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/coke.jpg" },
                new ItemMenu { Id = 2, ItemName = "Coke Zero", Price = 1.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/zero.jpg" },
                new ItemMenu { Id = 3, ItemName = "Ice Tea", Price = 0.59m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/tea.jpg" },
                new ItemMenu { Id = 4, ItemName = "Sprite", Price = 9m, ImagePath  = "avares://EBISX_POS/Assets/Images/Drinks/sprite.jpg" }
            };

            OptionsState.DrinkTypes.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasDrinks));
            };

            OptionsState.AddOnsType.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasAddOns));
            };
        }
    }
}
