using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ZendeskConfiguration _zendeskConfiguration;

        public TicketService(
            IHttpClientFactory httpClientFactory,
            ILogger<TicketService> logger,
            IOptions<ZendeskConfiguration> zendeskConfiguration)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //Note: check this last otherwise unit tests checking null ctor args will fail
            if (zendeskConfiguration is null)
                throw new ArgumentNullException(nameof(zendeskConfiguration));

            _zendeskConfiguration = zendeskConfiguration.Value ??
                                    throw new ArgumentNullException(
                                        $"{nameof(zendeskConfiguration)}.{nameof(zendeskConfiguration.Value)}",
                                        "zendeskConfiguration configuration value must not be null");
        }

        public async Task<CombinedTicket> GetTicket(long ticketId)
        {
            Console.WriteLine($"Getting ticket {ticketId}");

            var ticketJson = await GetTicketJson(ticketId, Sideloads.GetTicketSideloads());
            var ticketCommentJson = await GetTicketCommentsJson(ticketId);
            var ticketAuditsJson = await GetTicketAuditsJson(ticketId);

            _logger.LogInformation($"Ticket json: \n{ticketJson.PrettifyJsonString()}");
            _logger.LogInformation($"Ticket comments json: \n{ticketCommentJson.PrettifyJsonString()}");
            _logger.LogInformation($"Ticket audits json: \n{ticketAuditsJson.PrettifyJsonString()}");

            //TODO: Change prettifier to take doc as well as string, then use the following to convert all:
            //var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

            var zendeskTicketResponse = ticketJson.DeserializeZendeskTicketResponse();
            var zendeskTicket = ticketJson.DeserializeZendeskTicket();
            var comments = ticketCommentJson.DeserializeZendeskComments();
            var audits = ticketAuditsJson.DeserializeZendeskAudits();

            //TODO: Get the attachments

            return new CombinedTicket
            {
                Id = zendeskTicket.Id,
                Ticket = zendeskTicketResponse.Ticket,
                Users = zendeskTicketResponse.Users,
                Groups = zendeskTicketResponse.Groups,
                Organizations = zendeskTicketResponse.Organizations,
                Comments = comments,
                Audits = audits,
            };
        }

        public async Task<IDictionary<long, string>> GetTicketFields()
        {
            var jsonDoc = await GetTicketFieldsJsonDocument();

            if (jsonDoc != null)
            {
                var dictionary = jsonDoc.RootElement
                    .GetProperty("ticket_fields")
                    .EnumerateArray()
                    .Select(x =>
                        new
                        {
                            Key = x.SafeGetInt64("id"),
                            Value = x.SafeGetString("title")
                        })
                    .ToDictionary(t => t.Key,
                        t => t.Value);

                return dictionary;
                //var dict = TheTable.Select(t => new { t.Col1, t.Col2 })
                //    .ToDictionary(t => t.Col1, t => t);
            }

            return new Dictionary<long, string>();
        }

        public async Task<SafeTags> GetTicketTags(long ticketId)
        {
            var jsonDoc = await GetTicketJsonDocument(ticketId);

            if (jsonDoc != null)
            {
                var ticketElement =
                    jsonDoc.RootElement
                        .GetProperty("ticket");

                var createdAtString = ticketElement.SafeGetString("created_at");
                var updatedAtString = ticketElement.SafeGetString("updated_at");
                if (!DateTimeOffset.TryParse(updatedAtString, out var updatedAt))
                {
                    _logger.LogWarning($"Could not read updated-at date for ticket {ticketId}.");
                }

                var tags = ticketElement
                    .GetProperty("tags")
                    .EnumerateArray()
                    .Select(t => t.SafeGetString())
                    .ToList();
                //.EnumerateArray(
                //Extract tags only. with date
                var safeTags = new SafeTags
                {
                    Tags = tags,
                    SafeUpdate = true,
                    UpdatedStamp = updatedAt
                };

                return safeTags;
            }

            return null;
        }

        public async Task<IList<long>> SearchTickets()
        {
            //Look for tickets by
            //  ? form type
            //  ? created date
            //  ? no monitor_updated tag?

            /*
            https://support.zendesk.com/hc/en-us/articles/203663226-Zendesk-Support-search-reference#topic_ohr_wsc_3v
             type:ticket
            status<closed
            brand:tlevelsemployertest
            form:"T Levels - Employer Contact Form"
            searchParams = (
                query => 'type:ticket status:open',
                sort_by => 'created_at',
                sort_order => 'asc'
            );
            */
            // /search.json
            //Some of these search parameters will need to come from config
            var brandName = "tlevelsemployertest";
            //var query = $"brand:{brandName}";
            var formName = "T Levels - Employer Contact Form";

            //https://support.zendesk.com/hc/en-us/articles/203663226-Zendesk-Support-search-reference#topic_ohr_wsc_3v

            var query = $"type:ticket status:open form:{formName} brand:{brandName}";
            query = $"type:ticket status:open brand:{brandName}";
            query = $"type:ticket status:open brand:{brandName}&sort_by=created_by&sort_order=desc";
            query = $"type:ticket status:open brand:{brandName} order_by:created sort:desc";
            //query = "type:ticket status:open";
            //https://developer.zendesk.com/api-reference/ticketing/ticket-management/search/
            //query=created>2012-07-17 type:ticket organization:"MD Photo"

            var jsonDoc = await GetTicketSearchResultsJsonDocument(query);

            if (jsonDoc != null)
            {
                //foreach (var resultElement in jsonDoc.RootElement
                //    .GetProperty("results")
                //    .EnumerateArray())
                //{
                //    var id = resultElement.SafeGetInt64("id");
                //    ids.Add(id);
                //}
                //TODO: Worry about paging? Or somehow only get recent ones
                //Other fields:
                // subject = T Levels and industry placement support for employers - Online query
                //ticket_form_id=360001820480
                var count = jsonDoc.RootElement.GetProperty("count");
                var nextPage = jsonDoc.RootElement.GetProperty("next_page");
                _logger.LogInformation($"Search found {count} items. Next page is '{nextPage}'");
                var ids = jsonDoc.RootElement
                    .GetProperty("results")
                    .EnumerateArray()
                    .Select(x => x.SafeGetInt64("id"))
                    .OrderBy(x => x)
                    .ToList();

                return ids;
            }

            return new List<long>();
        }

        public async Task AddTag(long ticketId, CombinedTicket ticket, string tag)
        {
            Console.WriteLine($"Adding tag {tag} to ticket {ticketId}");

            if (ticket is null)
            {
                _logger.LogInformation("No ticket provided.");
                return;
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                _logger.LogInformation("No tag provided.");
                return;
            }

            //NOTE: Zendesk Monitor has a SafeModifyTags which gets a safe collection of the existing tags
            var currentTags = ticket.Ticket.Tags.ToList();

            currentTags.Add(tag);
            var updatedAt = ticket.Ticket?.UpdatedAt;
            //TODO: Needs to throw an exception if this wasn't found

            /*
             https://github.com/SkillsFundingAgency/das-zendesk-monitor/issues/32
             https://developer.zendesk.com/api-reference/ticketing/ticket-management/tags/#add-tags
             //PUT /api/v2/tickets/{ticket_id}/tags
            {
                "tags": ["customer"],
                "updated_stamp":"2019-09-12T21:45:16Z",
                "safe_update":"true"
            }
            For updated_stamp, retrieve and specify the ticket's latest updated_at timestamp. The tag update only occurs if the updated_stamp timestamp matches the ticket's actual updated_at timestamp at the time of the request. If the timestamps don't match (in other words, if the ticket was updated since you retrieved the ticket's last updated_at timestamp), the request returns a 409 Conflict error.
             */
            //2019-09-12T21:45:16Z
            //var formattedDate = $"{updatedAt:yyyy-MM-ddTHH:mm:ssZ}";

            var tagsToAdd = new SafeTags
            {
                Tags = new List<string> { tag },
                UpdatedStamp = updatedAt!.Value,
                SafeUpdate = true
            };

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //WriteIndented = true
            };
            var json = JsonSerializer.Serialize(tagsToAdd, serializerOptions);

            _logger.LogInformation($"Prepared json for adding ticket:\n{json}");

            //TODO: Add a PutJson method
            var httpClient = _httpClientFactory.CreateClient(nameof(TicketService));
            var requestUriFragment = $"tickets/{ticketId}/tags.json";
            _logger.LogInformation($"Calling Zendesk Support API {httpClient.BaseAddress} endpoint {requestUriFragment}");

            //var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            //var response = await httpClient.PutAsync(requestUriFragment, httpContent);

            var response = await httpClient.PutAsJsonAsync(requestUriFragment, tagsToAdd, serializerOptions);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            _logger.LogInformation($"Response from PUT tag: \n{jsonDoc.PrettifyJsonDocument()}");
        }

        private async Task<JsonDocument> GetTicketSearchResultsJsonDocument(string query)
        {
            var requestQueryString = $"?query={WebUtility.UrlEncode(query)}";

            return await GetJsonDocument($"search.json{requestQueryString}");
        }

        private async Task<string> GetTicketJson(long ticketId, string sideloads = null)
        {
            var requestQueryString = !string.IsNullOrEmpty(sideloads) ? $"?include={sideloads}" : "";
            return await GetJson($"tickets/{ticketId}.json{requestQueryString}");
        }

        private async Task<JsonDocument> GetTicketJsonDocument(long ticketId, string sideloads = null)
        {
            var requestQueryString = !string.IsNullOrEmpty(sideloads) ? $"?include={sideloads}" : "";
            return await GetJsonDocument($"tickets/{ticketId}.json{requestQueryString}");
        }

        private async Task<JsonDocument> GetTicketFieldsJsonDocument()
        {
            return await GetJsonDocument($"ticket_fields.json");
        }

        private async Task<string> GetTicketCommentsJson(long ticketId) =>
        await GetJson($"tickets/{ticketId}/comments.json");

        private async Task<string> GetTicketAuditsJson(long ticketId) =>
        await GetJson($"tickets/{ticketId}/audits.json");

        private async Task<string> GetJson(string requestUriFragment)
        {
            var content = await GetHttp(requestUriFragment);
            return await content.ReadAsStringAsync();
        }

        private async Task<JsonDocument> GetJsonDocument(string requestUriFragment)
        {
            var content = await GetHttp(requestUriFragment);
            return await JsonDocument.ParseAsync(await content.ReadAsStreamAsync());
        }

        private async Task<HttpContent> GetHttp(string requestUriFragment)
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(TicketService));

            _logger.LogInformation($"Calling Zendesk Support API {httpClient.BaseAddress} endpoint {requestUriFragment}");

            var response = await httpClient.GetAsync(requestUriFragment);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            return response.Content;
        }

    }
}
