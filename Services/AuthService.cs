using EBISX_POS.API.Services.DTO.Auth;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace EBISX_POS.Services
{
    public class AuthService
    {
        private readonly ApiSettings _apiSettings;
        private readonly RestClient _client;
        private readonly CookieContainer _cookieContainer;
        public AuthService(IOptions<ApiSettings> apiSettings, CookieContainer cookieContainer)
        {
            _apiSettings = apiSettings.Value;
            _cookieContainer = cookieContainer; // ✅ Use shared cookie container

            var options = new RestClientOptions(_apiSettings.LocalAPI.BaseUrl)
            {
                CookieContainer = _cookieContainer
            };

            _client = new RestClient(options);
        }


        public async Task<List<CashierDTO>> GetCashiersAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_apiSettings?.LocalAPI?.AuthEndpoint))
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var request = new RestRequest($"{_apiSettings.LocalAPI.AuthEndpoint}/Cashiers", Method.Get);
                var response = await _client.ExecuteAsync<List<CashierDTO>>(request);

                if (response.IsSuccessful && response.Data != null)
                {
                    return response.Data;
                }

                Debug.WriteLine($"HTTP Error: {response.ErrorMessage}");
                return new List<CashierDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return new List<CashierDTO>();
            }
        }

        public class LoginResponseDTO
        {
            public string CashierEmail { get; set; } = string.Empty;
            public string CashierName { get; set; } = string.Empty;
        }

        public async Task<(bool, string, string)> LogInAsync(LogInDTO logInDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiSettings?.LocalAPI?.AuthEndpoint))
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var request = new RestRequest($"{_apiSettings.LocalAPI.AuthEndpoint}/login", Method.Post)
                    .AddJsonBody(logInDTO);

                var response = await _client.ExecuteAsync<LoginResponseDTO>(request);

                if (response.IsSuccessful && response.Data != null)
                {
                    return (true, response.Data.CashierName, response.Data.CashierEmail);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return (false, "Invalid credentials. Please try again.", "");
                }

                return (false, $"Login failed. Status Code: {response.StatusCode}", "");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return (false, "Unexpected error occurred.", "");
            }
        }
        public class LogOutResponseDTO
        {
            public string Message { get; set; } = string.Empty;
        }

        public async Task<(bool, string)> LogOut(string managerEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiSettings?.LocalAPI?.AuthEndpoint))
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                // Build URL and create request with JSON body using PUT method
                var url = $"{_apiSettings.LocalAPI.AuthEndpoint}/LogOut";
                var request = new RestRequest(url, Method.Put)
                    .AddQueryParameter("cashierEmail", CashierState.CashierEmail)
                    .AddQueryParameter("managerEmail", managerEmail);


                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    return (true, response.Content ?? string.Empty);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Return the error message provided by the API.
                    return (false, response.Content ?? string.Empty);
                }

                return (false, $"LogOut failed. Status Code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return (false, "Unexpected error occurred.");
            }
        }

        public async Task<(bool, string, string)> HasPendingOrder()
        {
            try
            {
                if (string.IsNullOrEmpty(_apiSettings?.LocalAPI?.AuthEndpoint))
                {
                    throw new InvalidOperationException("API settings are not properly configured.");
                }

                var request = new RestRequest($"{_apiSettings.LocalAPI.AuthEndpoint}/HasPendingOrder", Method.Get);
                var response = await _client.ExecuteAsync<LoginResponseDTO>(request);

                if (response.IsSuccessful && response.Data != null)
                {
                    return (true, response.Data.CashierName, response.Data.CashierEmail);
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return (false, "No Pending Orders", "");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return (false, "Invalid credentials. Please try again.", "");
                }

                return (false, $"Request failed. Status Code: {response.StatusCode}", "");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return (false, "Unexpected error occurred.", "");
            }
        }
    }
}
