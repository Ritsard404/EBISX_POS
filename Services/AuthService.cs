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

        public AuthService(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
            _client = new RestClient(_apiSettings.LocalAPI.BaseUrl);
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
            public string CashierName { get; set; } = string.Empty;
        }

        public async Task<(bool, string)> LogInAsync(LogInDTO logInDTO)
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
                    return (true, response.Data.CashierName);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return (false, "Invalid credentials. Please try again.");
                }

                return (false, $"Login failed. Status Code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return (false, "Unexpected error occurred.");
            }
        }

        public async Task<(bool, string)> HasPendingOrder()
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
                    return (true, response.Data.CashierName);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return (false, "Invalid credentials. Please try again.");
                }

                return (false, $"Request failed. Status Code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                return (false, "Unexpected error occurred.");
            }
        }
    }
}
