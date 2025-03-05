using EBISX_POS.API.Models;
using EBISX_POS.Models; // Ensure this is added
using EBISX_POS.Services.DTO.Menu;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EBISX_POS.Services
{
    /// <summary>
    /// Service for handling menu-related operations including category, menu items, drinks, and add-ons
    /// </summary>
    public class MenuService
    {
        private readonly ApiSettings _apiSettings;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the MenuService
        /// </summary>
        /// <param name="apiSettings">Configuration settings for API access</param>
        /// <param name="httpClient">Optional HttpClient instance (for testing)</param>
        public MenuService(IOptions<ApiSettings> apiSettings, HttpClient? httpClient = null)
        {
            _apiSettings = apiSettings.Value;
            _httpClient = httpClient ?? new HttpClient(new HttpClientHandler { UseCookies = true });
        }

        /// <summary>
        /// Retrieves all menu categories from the API
        /// </summary>
        /// <returns>List of Category objects</returns>
        /// <exception cref="InvalidOperationException">Thrown if API settings are misconfigured</exception>
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                if (_apiSettings?.LocalAPI?.BaseUrl == null || _apiSettings.LocalAPI.AuthEndpoint == null)
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var url = $"{_apiSettings.LocalAPI.BaseUrl}/{_apiSettings.LocalAPI.MenuEndpoint}/Categories";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws exception if status code is not success

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<Category>>(jsonString, options) ?? new List<Category>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return new List<Category>(); // Return empty list on HTTP error
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Parsing Error: {ex.Message}");
                return new List<Category>(); // Return empty list if JSON parsing fails
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return new List<Category>(); // Catch any other unexpected errors
            }
        }

        /// <summary>
        /// Gets menu items for a specific category
        /// </summary>
        /// <param name="ctgryId">Category ID to filter menus</param>
        /// <returns>List of formatted ItemMenu objects</returns>
        /// <exception cref="InvalidOperationException">Thrown if API settings are misconfigured</exception>
        public async Task<List<ItemMenu>> GetMenusAsync(int ctgryId)
        {
            try
            {
                if (_apiSettings?.LocalAPI?.BaseUrl == null || _apiSettings.LocalAPI.AuthEndpoint == null)
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var url = $"{_apiSettings.LocalAPI.BaseUrl}/{_apiSettings.LocalAPI.MenuEndpoint}/Menus?ctgryId={ctgryId}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws exception if status code is not success

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var menus = JsonSerializer.Deserialize<List<Menu>>(jsonString, options) ?? new List<Menu>();

                // Map API response to ItemMenu model
                return menus.Select(menu => new ItemMenu
                {
                    Id = menu.Id,
                    ItemName = menu.MenuName ?? "Unknown",
                    Price = menu.MenuPrice,
                    ImagePath = menu.MenuImagePath ?? string.Empty,
                    Size = menu.Size?.ToString() ?? string.Empty,
                    HasSize = menu.Size != null,
                    IsSolo = !menu.HasDrink && menu.DrinkType == null && menu.IsAddOn == false,
                    IsAddOn = menu.IsAddOn,
                }).ToList();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return new List<ItemMenu>(); // Return empty list on HTTP error
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Parsing Error: {ex.Message}");
                return new List<ItemMenu>(); // Return empty list if JSON parsing fails
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return new List<ItemMenu>(); // Catch any other unexpected errors
            }
        }

        /// <summary>
        /// Retrieves available add-ons for a specific menu item
        /// </summary>
        /// <param name="menuId">Menu item ID to get add-ons for</param>
        /// <returns>List of AddOnTypeDTO objects</returns>
        /// <exception cref="InvalidOperationException">Thrown if API settings are misconfigured</exception>
        public async Task<List<AddOnTypeDTO>> GetAddOns(int menuId)
        {
            try
            {
                if (_apiSettings?.LocalAPI?.BaseUrl == null || _apiSettings.LocalAPI.AuthEndpoint == null)
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var url = $"{_apiSettings.LocalAPI.BaseUrl}/{_apiSettings.LocalAPI.MenuEndpoint}/AddOns?menuId={menuId}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<AddOnTypeDTO>>(jsonString, options)
                       ?? new List<AddOnTypeDTO>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return new List<AddOnTypeDTO>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Parsing Error: {ex.Message}");
                return new List<AddOnTypeDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return new List<AddOnTypeDTO>();
            }
        }

        /// <summary>
        /// Retrieves available drink options and sizes for a specific menu item
        /// </summary>
        /// <param name="menuId">Menu item ID to get drink options for</param>
        /// <returns>DrinksDTO containing drink types and available sizes</returns>
        /// <exception cref="InvalidOperationException">Thrown if API settings are misconfigured</exception>
        public async Task<DrinksDTO> GetDrinks(int menuId)
        {
            try
            {
                if (_apiSettings?.LocalAPI?.BaseUrl == null || _apiSettings.LocalAPI.AuthEndpoint == null)
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var url = $"{_apiSettings.LocalAPI.BaseUrl}/{_apiSettings.LocalAPI.MenuEndpoint}/Drinks?menuId={menuId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws if not successful

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var drinksDto = JsonSerializer.Deserialize<DrinksDTO>(jsonString, options);
                return drinksDto ?? new DrinksDTO
                {
                    DrinkTypesWithDrinks = new List<DrinkTypeDTO>(),
                    Sizes = new List<string>()
                };
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Parsing Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
            }

            // In case of an error, return an empty DrinksDTO
            return new DrinksDTO
            {
                DrinkTypesWithDrinks = new List<DrinkTypeDTO>(),
                Sizes = new List<string>()
            };
        }

    }
}
