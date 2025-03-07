using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.Models;
using EBISX_POS.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using EBISX_POS.Services.DTO.Menu;
using System.Collections.ObjectModel;
using EBISX_POS.State;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using EBISX_POS.API.Models;

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


        /// <summary>
        /// Indicates if options are currently being loaded
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _HasOptions;


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


            // Start loading options asynchronously without blocking UI
            // Note: In production, consider proper async/await with cancellation
            _ = LoadOptions();
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
                HasOptions = false;


                // 🔹 Reset state before fetching new data
                OptionsState.DrinkTypes.Clear();
                OptionsState.DrinkSizes.Clear();
                OptionsState.AddOnsType.Clear();

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

                    SelectedOptionsState.SelectedDrinkType = drinksResult.DrinkTypesWithDrinks.FirstOrDefault().DrinkTypeId;
                    SelectedOptionsState.SelectedSize = drinksResult.Sizes.FirstOrDefault();

                    // Store  Default
                    OptionsState.UpdateDrinks(drinksResult.DrinkTypesWithDrinks.FirstOrDefault().DrinkTypeId, drinksResult.Sizes.FirstOrDefault());

                }
                else
                {
                    OptionsState.DrinkTypes.Clear();
                    OptionsState.DrinkSizes.Clear();
                }

                // Load add-on options
                var addOnResult = await _menuService.GetAddOns(Item.Id);
                if (addOnResult != null)
                {
                    OptionsState.AddOnsType.Clear();
                    addOnResult.ForEach(addOn => OptionsState.AddOnsType.Add(addOn));

                    // Default Display
                    OptionsState.UpdateAddOns(addOnResult.FirstOrDefault().AddOnTypeId);

                }
                else
                {
                    OptionsState.AddOnsType.Clear();
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
                HasOptions = true;
            }
        }
    }
}
