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

        public async Task<ZInvoiceDTO> ZInvoiceReport() // Fixed return type
        {
            try
            {
                var url = $"{_apiSettings.LocalAPI.ReportEndpoint}/ZInvoiceReport";
                var request = new RestRequest(url, Method.Get);

                var response = await _restClient.ExecuteAsync<ZInvoiceDTO>(request);
                return response.Data ?? CreateDefaultZInvoice();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return CreateDefaultZInvoice();
            }
        }

        private ZInvoiceDTO CreateDefaultZInvoice()
        {
            return new ZInvoiceDTO
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
                BeginningSI = string.Empty,
                EndingSI = string.Empty,
                BeginningVoid = string.Empty,
                EndingVoid = string.Empty,
                BeginningReturn = string.Empty,
                EndingReturn = string.Empty,
                ResetCounter = string.Empty,
                ZCounter = string.Empty,
                PresentAccumulatedSales = "₱0.00",
                PreviousAccumulatedSales = "₱0.00",
                SalesForTheDay = "₱0.00",
                SalesBreakdown = new SalesBreakdown
                {
                    VatableSales = "₱0.00",
                    VatAmount = "₱0.00",
                    VatExemptSales = "₱0.00",
                    ZeroRatedSales = "₱0.00",
                    GrossAmount = "₱0.00",
                    LessDiscount = "₱0.00",
                    LessReturn = "₱0.00",
                    LessVoid = "₱0.00",
                    LessVatAdjustment = "₱0.00",
                    NetAmount = "₱0.00"
                },
                DiscountSummary = new DiscountSummary
                {
                    SeniorCitizen = "₱0.00",
                    Pwd = "₱0.00",
                    Other = "₱0.00"
                },
                SalesAdjustment = new SalesAdjustment
                {
                    Void = "₱0.00",
                    Return = "₱0.00"
                },
                VatAdjustment = new VatAdjustment
                {
                    ScTrans = "₱0.00",
                    PwdTrans = "₱0.00",
                    RegDiscTrans = "₱0.00",
                    ZeroRatedTrans = "₱0.00",
                    VatOnReturn = "₱0.00",
                    OtherAdjustments = "₱0.00"
                },
                TransactionSummary = new TransactionSummary
                {
                    CashInDrawer = "₱0.00",
                    OtherPayments = new List<OtherPayment>()
                },
                OpeningFund = "₱0.00",
                Withdrawal = "₱0.00",
                PaymentsReceived = "₱0.00",
                ShortOver = "₱0.00"
            };
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
                    Withdrawal = string.Empty,
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
                    Withdrawal = string.Empty,
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
