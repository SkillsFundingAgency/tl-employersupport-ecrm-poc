using System;
using System.Collections.Generic;
using System.Linq;
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
    public class EcrmODataApiClient : IEcrmODataApiClient
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly HttpClient _httpClient;
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger _logger;

        public EcrmODataApiClient(
            HttpClient httpClient,
            IAuthenticationService authenticationService,
            ILogger<EcrmODataApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Account> GetAccount(Guid accountId)
        {
            var query =
                $"$select=accountid,name,accountnumber,address1_primarycontactname,address1_line1,address1_line2,address1_line3,address1_postalcode,address1_city,telephone1,customersizecode&$orderby=name desc&$filter=accountid eq '{accountId:D}'";

            var request = await CreateRequest(HttpMethod.Get, $"accounts?{query}");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            
            _logger.LogDebug($"ECRM Account response json: \n{json.PrettifyJsonString()}");

            var accounts = JsonSerializer
                .Deserialize<AccountResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);

            return accounts.Accounts.FirstOrDefault();
        }

        public async Task<WhoAmIResponse> GetWhoAmI()
        {
            var request = await CreateRequest(HttpMethod.Get, "WhoAmI");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer
                .Deserialize<WhoAmIResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);
        }

        private async Task<HttpRequestMessage> CreateRequest(HttpMethod method, string requestUri)
        {
            var accessToken = await _authenticationService.GetAccessToken();

            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return request;
        }
    }
}
