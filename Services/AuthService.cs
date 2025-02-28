using EBISX_POS.Services.DTO.Auth;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EBISX_POS.Services
{
    public class AuthService
    {
        private readonly ApiSettings _apiSettings;
        private readonly HttpClient _httpClient;

        public AuthService(IOptions<ApiSettings> apiSettings, HttpClient? httpClient = null)
        {
            _apiSettings = apiSettings.Value;
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<List<CashierDTO>> GetCashiersAsync()
        {
            try
            {
                if (_apiSettings?.LocalAPI?.BaseUrl == null || _apiSettings.LocalAPI.AuthEndpoint == null)
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var url = $"{_apiSettings.LocalAPI.BaseUrl}/{_apiSettings.LocalAPI.AuthEndpoint}/Cashiers";
                Debug.WriteLine($"Constructed URL: {url}"); // Debug line

                var response = await _httpClient.GetAsync(url);

                Debug.WriteLine($"Raw JSON: {await response.Content.ReadAsStringAsync()}"); // Debug line

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not success

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<CashierDTO>>(jsonString, options) ?? new List<CashierDTO>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return new List<CashierDTO>(); // Return empty list on HTTP error
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Parsing Error: {ex.Message}");
                return new List<CashierDTO>(); // Return empty list if JSON parsing fails
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return new List<CashierDTO>(); // Catch any other unexpected errors
            }
        }
    }
}
