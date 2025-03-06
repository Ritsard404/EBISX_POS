using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EBISX_POS.API.Models; // Ensure this is added
using EBISX_POS.Services;
using System.Threading.Tasks; // Ensure this is added
using System.Diagnostics; // Ensure this is added

namespace EBISX_POS.ViewModels
{
    public class ItemListViewModel : ViewModelBase
    {
        private readonly MenuService _menuService;

        public ObservableCollection<ItemMenu> MenuItems { get; } = new();
        public ICommand ItemClickCommand { get; }

        public ItemListViewModel(MenuService menuService) // Ensure this constructor is public
        {
            _menuService = menuService;
            ItemClickCommand = new RelayCommand<ItemMenu>(OnItemClick);

        }

        public async Task LoadMenusAsync(int categoryId)
        {
            var menus = await _menuService.GetMenusAsync(categoryId);
            MenuItems.Clear();
            menus.ForEach(menu => MenuItems.Add(menu));
        }

        private void OnItemClick(ItemMenu? item)
        {
            if (item != null)
            {
                Debug.WriteLine($"Clicked: {item.Id}, Price: {item.Price}"); // Debugging
            }
        }
    }
}
