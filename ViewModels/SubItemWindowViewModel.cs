using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.Models;
using EBISX_POS.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using EBISX_POS.Services.DTO.Menu;
using System.Collections.ObjectModel;
using EBISX_POS.State;

namespace EBISX_POS.ViewModels
{
    /// <summary>
    /// ViewModel for managing the sub-item selection window, handling drink sizes and add-ons
    /// </summary>
    public partial class SubItemWindowViewModel : ViewModelBase
    {
        private readonly MenuService _menuService;

        /// <summary>
        /// The main menu item being customized
        /// </summary>
        public ItemMenu Item { get; }

        //// Observable collections for UI binding:
        ///// <summary>
        ///// Available drink types (e.g., Sodas, Juices)
        ///// </summary>
        //public ObservableCollection<DrinkTypeDTO> DrinkTypes { get; } = new ObservableCollection<DrinkTypeDTO>();
        
        ///// <summary>
        ///// Available drink sizes (e.g., Small, Medium, Large)
        ///// </summary>
        //public ObservableCollection<string> DrinkSizes { get; } = new ObservableCollection<string>();

        ///// <summary>
        ///// Available add-on categories (e.g., Sauces, Sides)
        ///// </summary>
        //public ObservableCollection<AddOnTypeDTO> AddOns { get; } = new ObservableCollection<AddOnTypeDTO>();

        /// <summary>
        /// Indicates if options are currently being loaded
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;


        public ObservableCollection<ItemMenu> OptionItems { get; set; }

        /// <summary>
        /// Initializes a new instance of the SubItemWindowViewModel
        /// </summary>
        /// <param name="item">The menu item being customized</param>
        /// <param name="menuService">Service for fetching menu data</param>
        /// <exception cref="ArgumentNullException">Thrown if any required service is null</exception>
        public SubItemWindowViewModel(ItemMenu item, MenuService menuService)
        {
            Item = item;
            _menuService = menuService;


            // Sample data
            OptionItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu { Id = 1, ItemName = "Coke", Price = 0.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/coke.jpg" },
                new ItemMenu { Id = 2, ItemName = "Coke Zero", Price = 1.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/zero.jpg" },
                new ItemMenu { Id = 3, ItemName = "Ice Tea", Price = 0.59m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/tea.jpg" },
                new ItemMenu { Id = 4, ItemName = "Sprite", Price = 9m, ImagePath  = "avares://EBISX_POS/Assets/Images/Drinks/sprite.jpg" }
            };

            // Start loading options asynchronously without blocking UI
            // Note: In production, consider proper async/await with cancellation
            _ = LoadOptions();
        }

        /// <summary>
        /// Initializes a new instance of the SubItemWindowViewModel with default values
        /// </summary>
        public SubItemWindowViewModel()
        {
            // Initialize with default values or sample data
            OptionItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu { Id = 1, ItemName = "Coke", Price = 0.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/coke.jpg" },
                new ItemMenu { Id = 2, ItemName = "Coke Zero", Price = 1.99m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/zero.jpg" },
                new ItemMenu { Id = 3, ItemName = "Ice Tea", Price = 0.59m, ImagePath = "avares://EBISX_POS/Assets/Images/Drinks/tea.jpg" },
                new ItemMenu { Id = 4, ItemName = "Sprite", Price = 9m, ImagePath  = "avares://EBISX_POS/Assets/Images/Drinks/sprite.jpg" }
            };
        }

        /// <summary>
        /// Loads available drink options and add-ons for the current menu item
        /// </summary>
        /// <remarks>
        /// 1. Checks for valid menu item
        /// 2. Shows loading indicator
        /// 3. Fetches drink options asynchronously
        /// 4. Fetches add-ons asynchronously
        /// 5. Updates observable collections for UI binding
        /// 6. Handles errors and updates loading state
        /// </remarks>
        public async Task LoadOptions()
        {
            try
            {
                // Validate required item
                if (Item == null)
                {
                    Debug.WriteLine("Error: Cannot load options - menu item is null");
                    return;
                }

                Debug.WriteLine($"Loading options for Item ID: {Item.Id}");
                IsLoading = true;

                // Load drink options
                var drinksResult = await _menuService.GetDrinks(Item.Id);
                if (drinksResult != null)
                {
                    // Update drink types
                    OptionsState.DrinkTypes.Clear();
                    foreach (var drinkType in drinksResult.DrinkTypesWithDrinks)
                    {
                        OptionsState.DrinkTypes.Add(drinkType);
                    }

                    // Update available sizes
                    OptionsState.DrinkSizes.Clear();
                    foreach (var size in drinksResult.Sizes)
                    {
                        OptionsState.DrinkSizes.Add(size);
                    }
                    Debug.WriteLine($"Successfully loaded {OptionsState.DrinkTypes.Count} drink types and {OptionsState.DrinkSizes.Count} sizes");
                }
                else
                {
                    Debug.WriteLine("No drink options available for this item");
                }

                // Load add-on options
                var addOnResult = await _menuService.GetAddOns(Item.Id);
                if (addOnResult != null)
                {
                    // Update add-ons collection
                    OptionsState.AddOns.Clear();
                    addOnResult.ForEach(addOn => OptionsState.AddOns.Add(addOn));
                    Debug.WriteLine($"Successfully loaded {OptionsState.AddOns.Count} add-on categories");
                }
                else
                {
                    Debug.WriteLine("No add-ons available for this item");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading options: {ex.Message}");
                // In production: Log error and show user-friendly message
            }
            finally
            {
                // Always clear loading state
                IsLoading = false;
            }
        }
    }
}
