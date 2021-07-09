using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        public async Task<Guid> CreateAccount(Account account)
        {
            var request = await CreateRequestWithToken(
                HttpMethod.Post, 
                "accounts",
                JsonSerializer.Serialize(
                    account, 
                    JsonExtensions.DefaultJsonSerializerOptions));

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            _logger.LogDebug($"ECRM Create account response json: \n{json.PrettifyJsonString()}");

            return account.AccountId!.Value;
        }

        public async Task<Guid> CreateContact(Contact contact)
        {
            var request = await CreateRequestWithToken(
                HttpMethod.Post,
                "contacts",
                JsonSerializer.Serialize(
                    contact,
                    JsonExtensions.IgnoreNullJsonSerializerOptions));

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            _logger.LogDebug($"ECRM Create contact response json: \n{json.PrettifyJsonString()}");

            var newId = contact.ContactId!.Value;
            
            foreach (var header in response.Headers)
            {
                Debug.WriteLine($"{header.Key} - {header.Value}");
                foreach (var value in header.Value)
                {
                    Debug.WriteLine($"{header.Key} - {header.Value}");
                }

                if (header.Key == "OData-EntityId")
                {
                     var found = Guid.TryParse(header.Value.First(), out newId);
                }
            }

            return newId; 
        }

        public async Task<Account> GetAccount(Guid accountId)
        {
            var request = await CreateRequestWithToken(HttpMethod.Get, $"accounts({accountId:D})");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            
            _logger.LogDebug($"ECRM Account response json: \n{json.PrettifyJsonString()}");

            var account = JsonSerializer
                .Deserialize<Account>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);

            return account;
        }
        
        public async Task<Account> SearchAccounts(Guid accountId)
        {
            var query = CreateAccountQuery($"accountid eq '{accountId:D}'");

            var request = await CreateRequestWithToken(HttpMethod.Get, $"accounts?{query}");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            _logger.LogDebug($"ECRM Account response json: \n{json.PrettifyJsonString()}");

            var accounts = JsonSerializer
                .Deserialize<AccountResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);

            return accounts?.Accounts.FirstOrDefault();
        }

        public async Task<WhoAmIResponse> GetWhoAmI()
        {
            var request = await CreateRequestWithToken(HttpMethod.Get, "WhoAmI");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer
                .Deserialize<WhoAmIResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);
        }

        [SuppressMessage("ReSharper", "CommentTypo")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private string CreateAccountQuery(string filter)
        {
            return "$select=accountid,name,accountnumber,address1_primarycontactname,address1_line1,address1_line2,address1_line3,address1_postalcode,address1_city,telephone1,customersizecode" +
                   "&$orderby=name desc" +
                   $"&$filter={filter}";
        }

        private async Task<HttpRequestMessage> CreateRequestWithToken(HttpMethod method, string requestUri, string body = null)
        {
            var accessToken = await _authenticationService.GetAccessToken();

            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (body != null)
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            return request;
        }
    }
}
