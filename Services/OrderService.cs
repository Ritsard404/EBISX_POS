using EBISX_POS.API.Services.DTO.Order;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using static EBISX_POS.Services.AuthService;

namespace EBISX_POS.Services
{
    public class OrderService
    {
        private readonly ApiSettings _apiSettings;
        private readonly RestClient _restClient; // Use RestClient instead of HttpClient

        // Constructor to initialize RestClient
        public OrderService(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;

            // Check if API settings are configured properly
            if (string.IsNullOrEmpty(_apiSettings?.LocalAPI?.BaseUrl))
            {
                throw new InvalidOperationException("API settings are not properly configured.");
            }

            // Initialize RestClient with BaseUrl
            _restClient = new RestClient(_apiSettings.LocalAPI.BaseUrl);
        }

        // AddCurrentOrderVoid Using RestSharp
        public async Task<(bool, string)> AddCurrentOrderVoid(AddCurrentOrderVoidDTO voidOrder)
        {
            try
            {
                // Validate API configuration
                if (string.IsNullOrEmpty(_apiSettings.LocalAPI.OrderEndpoint))
                {
                    throw new InvalidOperationException("Order endpoint is not configured.");
                }

                // Prepare API URL and RestSharp request
                var url = $"{_apiSettings.LocalAPI.OrderEndpoint}/AddCurrentOrderVoid";
                var request = new RestRequest(url, Method.Post).AddJsonBody(voidOrder);

                // Execute Request
                var response = await _restClient.ExecuteAsync(request);

                // Handle Success Response (200 OK)
                if (response.IsSuccessful)
                {
                    return (true, response.Content ?? "Order voided successfully.");
                }

                //  Handle Error Responses
                return response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => (false, response.Content ?? "Invalid request."),
                    HttpStatusCode.Unauthorized => (false, "Unauthorized access. Please check your credentials."),
                    _ => (false, $"Request failed with status code: {response.StatusCode}")
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error: {ex.Message}");
                return (false, "An unexpected error occurred.");
            }
        }
    }
}
