using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EBISX_POS.ViewModels
{
    public class ItemListViewModel : ViewModelBase
    {
        public ObservableCollection<ItemMenu> MenuItems { get; set; }
        public ICommand ItemClickCommand { get; }

        public ItemListViewModel()
        {
            // Sample data
            MenuItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 1, ItemName = "Burger Deluxe", Price = 5.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Burgers/burger.png" },
                new ItemMenu { Id = 2, ItemName = "French Fries", Price = 2.99m, ImagePath  = "avares://EBISX_POS/Assets/Images/Burgers/Yumburger-Solo.png" }
            };

            ItemClickCommand = new RelayCommand<ItemMenu>(OnItemClick);
        }

        private void OnItemClick(ItemMenu? item)
        {
            if (item != null)
            {
                Console.WriteLine($"Clicked: {item.ItemName}, Price: {item.Price}");
                System.Diagnostics.Debug.WriteLine($"Clicked: {item.ItemName}, Price: {item.Price}"); // Debugging
            }
        }


    }
}
