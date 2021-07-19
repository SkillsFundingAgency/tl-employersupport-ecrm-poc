using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.ApiClients
{
    public class EcrmApiClient : IEcrmApiClient
    {
        private readonly HttpClient _httpClient;
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger _logger;
        private readonly EcrmConfiguration _configuration;

        public EcrmApiClient(
            HttpClient httpClient,
            IOptions<EcrmConfiguration> configuration,
            ILogger<EcrmApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _configuration = configuration?.Value ??
                             throw new ArgumentNullException(
                                 $"{nameof(configuration)}",
                                 "Configuration or configuration value must not be null");
        }

        public async Task<Employer> GetEmployer(EmployerSearchRequest searchRequest)
        {
            var requestQueryString = JsonSerializer.Serialize(searchRequest, JsonExtensions.CamelCaseJsonSerializerOptions);

            var content = await _httpClient.PostAsJson("employerSearch", searchRequest);

            var json = await content.ReadAsStringAsync();

            return JsonSerializer
                .Deserialize<Employer>(
                    json,
                    JsonExtensions.CamelCaseJsonSerializerOptions);
        }

        public async Task<Account> GetAccount(Guid accountId)
        {
            //TODO: Just create request and call API - put back default headers etc in program
            //var request = CreateRequest(HttpMethod.Get, $"accounts({accountId:D})");
            //var response = await _httpClient.SendAsync(request);

            //Debug.WriteLine($"Get account response status {response.StatusCode}");
            //response.EnsureSuccessStatusCode();

            ////TODO: Deserialize to account

            _httpClient.BaseAddress = _configuration.ApiBaseUri.EndsWith("/")
                    ? new Uri(_configuration.ApiBaseUri)
                    : new Uri(_configuration.ApiBaseUri + "/");

            var heartbeatRequest = new HttpRequestMessage(HttpMethod.Get, "HeartBeat");
            heartbeatRequest.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            heartbeatRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            heartbeatRequest.Headers.Add("Ocp-Apim-Trace", "true");
            heartbeatRequest.Headers.Add("Ocp-Apim-Subscription-Key", _configuration.ApiKey);
            var heartbeatResponse = await _httpClient.SendAsync(heartbeatRequest);
            Debug.WriteLine($"Heartbeat response status {heartbeatResponse.StatusCode}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"accounts({accountId:D})");
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Ocp-Apim-Trace", "true");
            request.Headers.Add("Ocp-Apim-Subscription-Key", _configuration.ApiKey);

            var response = await _httpClient.SendAsync(request);

            Debug.WriteLine($"Get account response status {response.StatusCode}");
            response.EnsureSuccessStatusCode();

            //TODO: Deserialize
            return new Account();
        }

        public async Task<bool> GetHeartbeat()
        {
            var request = CreateRequest(HttpMethod.Get, "HeartBeat");

            var response = await _httpClient.SendAsync(request);

            //var response = await _httpClient.GetAsync("HeartBeat");
            return response.StatusCode == HttpStatusCode.OK;
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, string requestUri, bool trace = true)
        {
            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            if (trace)
                request.Headers.Add("Ocp-Apim-Trace", "true");

            return request;
        }
    }
}
