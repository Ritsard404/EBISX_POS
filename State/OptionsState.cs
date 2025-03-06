using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.Services.DTO.Menu;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace EBISX_POS.State
{
    public static class OptionsState
    {
        public static ObservableCollection<DrinkTypeDTO> DrinkTypes { get; set; } = new ObservableCollection<DrinkTypeDTO>();
        public static ObservableCollection<DrinkDetailDTO> Drinks { get; set; } = new ObservableCollection<DrinkDetailDTO>();
        public static ObservableCollection<string> DrinkSizes { get; set; } = new ObservableCollection<string>();
        public static ObservableCollection<AddOnTypeDTO> AddOnsType { get; set; } = new ObservableCollection<AddOnTypeDTO>();
        public static ObservableCollection<AddOnDetailDTO> AddOns { get; set; } = new ObservableCollection<AddOnDetailDTO>();


        public static void UpdateDrinks(int drinkTypeId)
        {
            Drinks.Clear();
            var drinks = DrinkTypes.FirstOrDefault(d => d.DrinkTypeId == drinkTypeId)?.Drinks;
            if (drinks != null)
            {
                foreach (var drink in drinks)
                {
                    Drinks.Add(drink);
                }
            }
        }

        public static void UpdateAddOns(int addOnTypeId)
        {
            AddOns.Clear();
            var addOns = AddOnsType.FirstOrDefault(a => a.AddOnTypeId == addOnTypeId)?.AddOns;
            if (addOns != null)
            {
                foreach (var addOn in addOns)
                {
                    addOn.HasSize = !string.IsNullOrEmpty(addOn.Size);
                    AddOns.Add(addOn);
                }
            }
        }
    }
}
