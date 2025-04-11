using EBISX_POS.API.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace EBISX_POS.Services
{
    public class ReportService
    {
        private readonly ApiSettings _apiSettings;
        private readonly RestClient _restClient;
        public ReportService(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;

            _restClient = new RestClient(_apiSettings.LocalAPI.BaseUrl);
        }


        public class CashTrackResponse
        {
            public string CashInDrawer { get; set; } = string.Empty;

            public string CurrentCashDrawer { get; set; } = string.Empty;
        }

        public async Task<(string CashInDrawer, string CurrentCashDrawer)> CashTrack()
        {
            try
            {
                // Build URL and create a GET request
                var url = $"{_apiSettings.LocalAPI.ReportEndpoint}/CashTrack";
                var request = new RestRequest(url, Method.Get)
                    .AddQueryParameter("cashierEmail", CashierState.CashierEmail);

                // Execute the request and return the data, or an empty list if null
                var response = await _restClient
                    .ExecuteAsync<CashTrackResponse>(request);


                if (!response.IsSuccessful || response.Data == null)
                {
                    Debug.WriteLine(
                      $"[CashTrack] HTTP {(int)response.StatusCode}: {response.ErrorMessage}"
                    );
                    return (string.Empty, string.Empty);
                }

                return (
                    response.Data.CashInDrawer,
                    response.Data.CurrentCashDrawer
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return (string.Empty, string.Empty);
            }
        }
    }
}
