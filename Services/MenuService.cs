using EBISX_POS.API.Models;
using EBISX_POS.Models; // Ensure this is added
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
    public class MenuService
    {
        private readonly ApiSettings _apiSettings;
        private readonly HttpClient _httpClient;

        public MenuService(IOptions<ApiSettings> apiSettings, HttpClient? httpClient = null)
        {
            _apiSettings = apiSettings.Value;
            _httpClient = httpClient ?? new HttpClient(new HttpClientHandler { UseCookies = true });
        }

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
                    IsSolo = !menu.HasDrink && menu.DrinkType == null && menu.IsAddOn == false
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
    }
}
