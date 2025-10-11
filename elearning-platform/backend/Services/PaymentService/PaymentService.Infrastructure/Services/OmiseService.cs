using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PaymentService.Infrastructure.Services
{
    /// <summary>
    /// Omise payment gateway service
    /// </summary>
    public class OmiseService : IOmiseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly string _publicKey;
        private const string BaseUrl = "https://api.omise.co";

        public OmiseService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = configuration["Omise:SecretKey"];
            _publicKey = configuration["Omise:PublicKey"];

            // Set authorization header
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_secretKey}:"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<OmiseChargeResponse> CreateChargeAsync(OmiseChargeRequest request)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("amount", (request.AmountThb * 100).ToString("0")), // Amount in satangs
                    new KeyValuePair<string, string>("currency", "THB"),
                    new KeyValuePair<string, string>("description", request.Description),
                    new KeyValuePair<string, string>("card", request.CardToken),
                    new KeyValuePair<string, string>("return_uri", request.ReturnUri ?? ""),
                    new KeyValuePair<string, string>("metadata[order_id]", request.OrderId.ToString()),
                    new KeyValuePair<string, string>("metadata[user_id]", request.UserId.ToString())
                });

                var response = await _httpClient.PostAsync($"{BaseUrl}/charges", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Omise API error: {responseContent}");
                }

                return JsonConvert.DeserializeObject<OmiseChargeResponse>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create charge: {ex.Message}", ex);
            }
        }

        public async Task<OmiseChargeResponse> GetChargeAsync(string chargeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/charges/{chargeId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Omise API error: {responseContent}");
                }

                return JsonConvert.DeserializeObject<OmiseChargeResponse>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get charge: {ex.Message}", ex);
            }
        }

        public async Task<OmiseRefundResponse> CreateRefundAsync(string chargeId, decimal amountThb)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("amount", (amountThb * 100).ToString("0")) // Amount in satangs
                });

                var response = await _httpClient.PostAsync($"{BaseUrl}/charges/{chargeId}/refunds", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Omise API error: {responseContent}");
                }

                return JsonConvert.DeserializeObject<OmiseRefundResponse>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create refund: {ex.Message}", ex);
            }
        }

        public async Task<OmiseCustomerResponse> CreateCustomerAsync(OmiseCustomerRequest request)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", request.Email),
                    new KeyValuePair<string, string>("description", request.Description),
                    new KeyValuePair<string, string>("card", request.CardToken)
                });

                var response = await _httpClient.PostAsync($"{BaseUrl}/customers", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Omise API error: {responseContent}");
                }

                return JsonConvert.DeserializeObject<OmiseCustomerResponse>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create customer: {ex.Message}", ex);
            }
        }

        public bool VerifyWebhookSignature(string payload, string signature)
        {
            // Implement webhook signature verification
            // This is a placeholder - implement actual HMAC verification
            return true;
        }
    }

    public interface IOmiseService
    {
        Task<OmiseChargeResponse> CreateChargeAsync(OmiseChargeRequest request);
        Task<OmiseChargeResponse> GetChargeAsync(string chargeId);
        Task<OmiseRefundResponse> CreateRefundAsync(string chargeId, decimal amountThb);
        Task<OmiseCustomerResponse> CreateCustomerAsync(OmiseCustomerRequest request);
        bool VerifyWebhookSignature(string payload, string signature);
    }

    // Request/Response models
    public class OmiseChargeRequest
    {
        public decimal AmountThb { get; set; }
        public string Description { get; set; }
        public string CardToken { get; set; }
        public string ReturnUri { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
    }

    public class OmiseChargeResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } // pending, successful, failed

        [JsonProperty("paid")]
        public bool Paid { get; set; }

        [JsonProperty("authorized")]
        public bool Authorized { get; set; }

        [JsonProperty("transaction")]
        public string Transaction { get; set; }

        [JsonProperty("failure_code")]
        public string FailureCode { get; set; }

        [JsonProperty("failure_message")]
        public string FailureMessage { get; set; }

        [JsonProperty("card")]
        public OmiseCard Card { get; set; }

        [JsonProperty("metadata")]
        public OmiseMetadata Metadata { get; set; }
    }

    public class OmiseCard
    {
        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("last_digits")]
        public string LastDigits { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("expiration_month")]
        public int ExpirationMonth { get; set; }

        [JsonProperty("expiration_year")]
        public int ExpirationYear { get; set; }
    }

    public class OmiseMetadata
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    public class OmiseRefundResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("charge")]
        public string Charge { get; set; }

        [JsonProperty("transaction")]
        public string Transaction { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class OmiseCustomerRequest
    {
        public string Email { get; set; }
        public string Description { get; set; }
        public string CardToken { get; set; }
    }

    public class OmiseCustomerResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("default_card")]
        public string DefaultCard { get; set; }

        [JsonProperty("cards")]
        public OmiseCardList Cards { get; set; }
    }

    public class OmiseCardList
    {
        [JsonProperty("data")]
        public OmiseCard[] Data { get; set; }
    }
}