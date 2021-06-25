using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.ApiClients
{
    public class ZendeskApiClient : IZendeskApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public ZendeskApiClient(
            HttpClient httpClient,
            ILogger<ZendeskApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetTicketJson(long ticketId, string sideloads = null)
        {
            var requestQueryString = !string.IsNullOrEmpty(sideloads) ? $"?include={sideloads}" : "";
            return await GetJson($"tickets/{ticketId}.json{requestQueryString}");
        }

        public async Task<JsonDocument> GetTicketJsonDocument(long ticketId, string sideloads = null)
        {
            var requestQueryString = !string.IsNullOrEmpty(sideloads) ? $"?include={sideloads}" : "";
            return await GetJsonDocument($"tickets/{ticketId}.json{requestQueryString}");
        }

        public async Task<string> GetTicketCommentsJson(long ticketId) =>
            await GetJson($"tickets/{ticketId}/comments.json");

        public async Task<string> GetTicketAuditsJson(long ticketId) =>
            await GetJson($"tickets/{ticketId}/audits.json");

        public async Task<JsonDocument> GetTicketFieldsJsonDocument() =>
            await GetJsonDocument("ticket_fields.json");

        public async Task<JsonDocument> GetTicketSearchResultsJsonDocument(string query)
        {
            var requestQueryString = $"query={WebUtility.UrlEncode(query)}";

            return await GetJsonDocument($"search.json?{requestQueryString}");
        }

        public async Task<JsonDocument> PostTags(long ticketId, SafeTags tags)
        {
            var requestUriFragment = $"tickets/{ticketId}/tags.json";
            _logger.LogInformation($"Calling Zendesk Support API {_httpClient.BaseAddress} endpoint {requestUriFragment}");

            var response = await _httpClient.PostAsJsonAsync(requestUriFragment, tags, JsonExtensions.DefaultJsonSerializerOptions);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            return jsonDocument;
        }

        public async Task<JsonDocument> PutTags(long ticketId, SafeTags tags)
        {
            var requestUriFragment = $"tickets/{ticketId}/tags.json";
            _logger.LogInformation($"Calling Zendesk Support API {_httpClient.BaseAddress} endpoint {requestUriFragment}");

            var response = await _httpClient.PutAsJsonAsync(requestUriFragment, tags, JsonExtensions.DefaultJsonSerializerOptions);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            return jsonDocument;
        }

        private async Task<string> GetJson(string requestUri)
        {
            var content = await GetHttp(requestUri);
            return await content.ReadAsStringAsync();
        }

        private async Task<JsonDocument> GetJsonDocument(string requestUri)
        {
            var content = await GetHttp(requestUri);
            return await JsonDocument.ParseAsync(await content.ReadAsStreamAsync());
        }

        private async Task<HttpContent> GetHttp(string requestUri)
        {
            _logger.LogInformation($"Calling Zendesk Support API {_httpClient.BaseAddress} endpoint {requestUri}");

            var response = await _httpClient.GetAsync(requestUri);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            return response.Content;
        }
    }
}
