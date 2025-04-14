using Microsoft.Extensions.Options;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using EBISX_POS.Services.DTO.Report;

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

        public async Task<XInvoiceDTO> XInvoiceReport()
        {
            try
            {
                var url = $"{_apiSettings.LocalAPI.ReportEndpoint}/XInvoiceReport";
                var request = new RestRequest(url, Method.Get);

                var response = await _restClient.ExecuteAsync<XInvoiceDTO>(request);
                return response.Data ?? new XInvoiceDTO
                {
                    BusinessName = string.Empty,
                    OperatorName = string.Empty,
                    AddressLine = string.Empty,
                    VatRegTin = string.Empty,
                    Min = string.Empty,
                    SerialNumber = string.Empty,
                    ReportDate = string.Empty,
                    ReportTime = string.Empty,
                    StartDateTime = string.Empty,
                    EndDateTime = string.Empty,
                    Cashier = string.Empty,
                    BeginningOrNumber = string.Empty,
                    EndingOrNumber = string.Empty,
                    OpeningFund = string.Empty,
                    Payments = new Payment(),
                    VoidAmount = string.Empty,
                    Refund = string.Empty,
                    TransactionSummary = new TransactionSummary
                    {
                        CashInDrawer = string.Empty,
                        OtherPayments = new List<OtherPayment>()
                    },
                    ShortOver = string.Empty
                }; ;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return new XInvoiceDTO
                {
                    BusinessName = string.Empty,
                    OperatorName = string.Empty,
                    AddressLine = string.Empty,
                    VatRegTin = string.Empty,
                    Min = string.Empty,
                    SerialNumber = string.Empty,
                    ReportDate = string.Empty,
                    ReportTime = string.Empty,
                    StartDateTime = string.Empty,
                    EndDateTime = string.Empty,
                    Cashier = string.Empty,
                    BeginningOrNumber = string.Empty,
                    EndingOrNumber = string.Empty,
                    OpeningFund = string.Empty,
                    Payments = new Payment(),
                    VoidAmount = string.Empty,
                    Refund = string.Empty,
                    TransactionSummary = new TransactionSummary
                    {
                        CashInDrawer = string.Empty,
                        OtherPayments = new List<OtherPayment>()
                    },
                    ShortOver = string.Empty
                };
            }
        }
    }
}
