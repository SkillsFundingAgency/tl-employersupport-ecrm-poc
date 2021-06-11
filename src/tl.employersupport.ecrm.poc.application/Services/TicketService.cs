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
            var formattedDate = $"{updatedAt:yyyy-MM-ddTHH:mm:ssZ}";
            var jsonAsString =
                "{\n" +
                    $"\"tags\": [\"{tag}\"],\n" +
                    $"\"updated_stamp\":\"{formattedDate}\",\n" +
                    "\"safe_update\":\"true\"\n" +
                "}";

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

            //_logger.LogInformation($"Calling Zendesk Support API {httpClient.BaseAddress} endpoint {requestUriFragment}");
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

        private async Task<string> GetTicketJson(long ticketId, string sideloads)
        {
            var requestQueryString = !string.IsNullOrEmpty(sideloads) ? $"?include={sideloads}" : "";
            return await GetJson($"tickets/{ticketId}.json{requestQueryString}");
        }

        private async Task<string> GetTicketCommentsJson(long ticketId) =>
        await GetJson($"tickets/{ticketId}/comments.json");

        private async Task<string> GetTicketAuditsJson(long ticketId) =>
        await GetJson($"tickets/{ticketId}/audits.json");

        private async Task<string> GetJson(string requestUriFragment)
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(TicketService));

            _logger.LogInformation($"Calling Zendesk Support API {httpClient.BaseAddress} endpoint {requestUriFragment}");

            var response = await httpClient.GetAsync(requestUriFragment);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}
