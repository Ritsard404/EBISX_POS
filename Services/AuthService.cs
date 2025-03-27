using EBISX_POS.API.Services.DTO.Auth;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
            _httpClient = httpClient ?? new HttpClient(new HttpClientHandler { UseCookies = true });
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
               
                var response = await _httpClient.GetAsync(url);
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


        public class LoginResponseDTO
        {
            public string CashierName { get; set; } = string.Empty;
        }

        public async Task<(bool, string)> LogInAsync(LogInDTO logInDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiSettings?.LocalAPI?.BaseUrl) || string.IsNullOrEmpty(_apiSettings.LocalAPI.AuthEndpoint))
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var url = $"{_apiSettings.LocalAPI.BaseUrl}/{_apiSettings.LocalAPI.AuthEndpoint}/login";
                var response = await _httpClient.PostAsJsonAsync(url, logInDTO);

                // ✅ Handle Success Response (200)
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<LoginResponseDTO>(jsonString, options);

                    string cashierName = result?.CashierName ?? "Unknown";
                    Debug.WriteLine($"Login Successful! Welcome, {cashierName}.");
                    return (true, $"Login Successful! Welcome, {cashierName}.");
                }

                // ❌ Handle Unauthorized Response (401)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return (false, "Invalid credentials. Please try again.");
                }

                return (false, $"Login failed. Status Code: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return (false, "HTTP Error"); // Return empty list on HTTP error
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Parsing Error: {ex.Message}");
                return (false, "JSON Parsing Error"); // Return empty list if JSON parsing fails
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return (false, "Unexpected error occurred.");
            }

        }

    }
}
