using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.Services.DTO.Menu;
using System.Collections.ObjectModel;
using System.Linq;

namespace EBISX_POS.State
{
    public static class OptionsState
    {
        public static ObservableCollection<DrinkTypeDTO> DrinkTypes { get; } = new ObservableCollection<DrinkTypeDTO>();

        public static ObservableCollection<string> DrinkSizes { get; } = new ObservableCollection<string>();
        public static ObservableCollection<AddOnTypeDTO> AddOns { get; } = new ObservableCollection<AddOnTypeDTO>();

    }
}
