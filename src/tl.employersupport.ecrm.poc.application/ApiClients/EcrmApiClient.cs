using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.ApiClients
{
    public class EcrmApiClient : IEcrmApiClient
    {
        private readonly HttpClient _httpClient;
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger _logger;

        public EcrmApiClient(
            HttpClient httpClient,
            ILogger<EcrmApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        //public async Task<bool> GetAccount(Guid accountId)
        //{
        //    var request = CreateRequest(HttpMethod.Get, "HeartBeat");
        //
        //    var response = await _httpClient.SendAsync(request);
        //
        //    return response.StatusCode == HttpStatusCode.OK;
        //}

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
