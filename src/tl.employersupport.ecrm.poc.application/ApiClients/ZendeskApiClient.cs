using System;
using System.Net;
using System.Net.Http;
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
        // ReSharper disable once NotAccessedField.Local
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
            return await _httpClient.GetJson($"tickets/{ticketId}.json{requestQueryString}");
        }

        public async Task<JsonDocument> GetTicketJsonDocument(long ticketId, string sideloads = null)
        {
            var requestQueryString = !string.IsNullOrEmpty(sideloads) ? $"?include={sideloads}" : "";
            return await _httpClient.GetJsonDocument($"tickets/{ticketId}.json{requestQueryString}");
        }

        public async Task<string> GetTicketCommentsJson(long ticketId) =>
            await _httpClient.GetJson($"tickets/{ticketId}/comments.json");

        public async Task<string> GetTicketAuditsJson(long ticketId) =>
            await _httpClient.GetJson($"tickets/{ticketId}/audits.json");

        public async Task<JsonDocument> GetTicketFieldsJsonDocument() =>
            await _httpClient.GetJsonDocument("ticket_fields.json");

        public async Task<JsonDocument> GetTicketSearchResultsJsonDocument(string query) =>
            await _httpClient.GetJsonDocument($"search.json?query={WebUtility.UrlEncode(query)}");
        
        public async Task<JsonDocument> PostTags(long ticketId, SafeTags tags)
        {
            var content = await _httpClient.PostAsJson($"tickets/{ticketId}/tags.json", tags);
            var jsonDocument = await JsonDocument.ParseAsync(await content.ReadAsStreamAsync());
            return jsonDocument;
        }

        public async Task<JsonDocument> PutTags(long ticketId, SafeTags tags)
        {
            var content = await _httpClient.PutAsJson($"tickets/{ticketId}/tags.json", tags);
            var jsonDocument = await JsonDocument.ParseAsync(await content.ReadAsStreamAsync());
            return jsonDocument;
        }
    }
}
