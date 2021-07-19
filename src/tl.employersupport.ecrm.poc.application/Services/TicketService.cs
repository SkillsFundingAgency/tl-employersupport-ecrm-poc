using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ILogger _logger;
        private readonly IZendeskApiClient _zendeskApiClient;
        // ReSharper disable once NotAccessedField.Local
        private readonly ZendeskConfiguration _zendeskConfiguration;

        public TicketService(
            IZendeskApiClient zendeskApiClient,
            ILogger<TicketService> logger,
            IOptions<ZendeskConfiguration> zendeskConfiguration)
        {
            _zendeskApiClient = zendeskApiClient ?? throw new ArgumentNullException(nameof(zendeskApiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _zendeskConfiguration = zendeskConfiguration?.Value ??
                                    throw new ArgumentNullException(
                                        $"{nameof(zendeskConfiguration)}",
                                        "Configuration or configuration value must not be null");
        }

        public async Task<EmployerContactTicket> GetEmployerContactTicket(long ticketId)
        {
            _logger.LogInformation($"Getting ticket {ticketId}");

            var jsonDocument = await _zendeskApiClient.GetTicketJsonDocument(ticketId, Sideloads.GetTicketSideloads());

            if (jsonDocument != null)
            {
                //var temp = jsonDocument.PrettifyJsonDocument();

                var fieldDefinitions = await GetTicketFields();

                var ticket = jsonDocument.ToEmployerContactTicket(fieldDefinitions);
                return ticket;
            }

            return null;
        }

        public async Task<CombinedTicket> GetTicket(long ticketId)
        {
            _logger.LogInformation($"Getting ticket {ticketId}");

            var ticketJson = await _zendeskApiClient.GetTicketJson(ticketId, Sideloads.GetTicketSideloads());
            var ticketCommentJson = await _zendeskApiClient.GetTicketCommentsJson(ticketId);
            var ticketAuditsJson = await _zendeskApiClient.GetTicketAuditsJson(ticketId);

            _logger.LogDebug($"Ticket json: \n{ticketJson.PrettifyJsonString()}");
            _logger.LogDebug($"Ticket comments json: \n{ticketCommentJson.PrettifyJsonString()}");
            _logger.LogDebug($"Ticket audits json: \n{ticketAuditsJson.PrettifyJsonString()}");

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

        public async Task<TicketFieldCollection> GetTicketFields()
        {
            var jsonDocument = await _zendeskApiClient.GetTicketFieldsJsonDocument();

            if (jsonDocument != null)
            {
                var count = jsonDocument.RootElement.GetProperty("count");
                var nextPage = jsonDocument.RootElement.GetProperty("next_page");
                _logger.LogInformation($"GetTicketFields found {count} items. Next page is '{nextPage}'");

                return jsonDocument.DeserializeTicketFields();
            }

            return new TicketFieldCollection();
        }

        public async Task<SafeTags> GetTicketTags(long ticketId)
        {
            var jsonDocument = await _zendeskApiClient.GetTicketJsonDocument(ticketId);

            return jsonDocument?.ExtractTicketSafeTags();
        }

        public async Task<IList<TicketSearchResult>> SearchTickets()
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
            //var formName = "T Levels - Employer Contact Form";

            //https://support.zendesk.com/hc/en-us/articles/203663226-Zendesk-Support-search-reference#topic_ohr_wsc_3v

            //var query = $"type:ticket status:open form:{formName} brand:{brandName}";
            var query = $"type:ticket status:new brand:{brandName}";
            //query = $"type:ticket status:open brand:{brandName}&sort_by=created_by&sort_order=desc";
            //query = $"type:ticket status:open brand:{brandName} order_by:created sort:desc";
            //query = "type:ticket status:open";
            //https://developer.zendesk.com/api-reference/ticketing/ticket-management/search/
            //query=created>2012-07-17 type:ticket organization:"MD Photo"

            var jsonDocument = await _zendeskApiClient.GetTicketSearchResultsJsonDocument(query);

            if (jsonDocument != null)
            {
                //var temp =jsonDocument.PrettifyJsonDocument();
                //TODO: Worry about paging? Or only get recent ones?
                var count = jsonDocument.RootElement.GetProperty("count");
                var nextPage = jsonDocument.RootElement.GetProperty("next_page");
                _logger.LogInformation($"Search found {count} items. Next page is '{nextPage}'");
                var searchResults = jsonDocument.ToTicketSearchResultList();
                return searchResults;
            }

            return new List<TicketSearchResult>();
        }

        public async Task AddTag(long ticketId, CombinedTicket ticket, string tag)
        {
            _logger.LogInformation($"Adding tag {tag} to ticket {ticketId}");

            if (ticket is null)
            {
                _logger.LogWarning("No ticket provided.");
                return;
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                _logger.LogWarning("No tag provided.");
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

            var tags = new SafeTags
            {
                Tags = new List<string> { tag },
                UpdatedStamp = updatedAt!.Value,
                SafeUpdate = true
            };

            var jsonDocument = await _zendeskApiClient.PutTags(ticketId, tags);

            _logger.LogInformation($"Response from PUT tag: \n{jsonDocument.PrettifyJsonDocument()}");
        }

        public async Task ModifyTags(long ticketId, SafeTags tags)
        {
            _logger.LogInformation($"Adding tags for ticket {ticketId}");

            if (tags is null)
            {
                _logger.LogWarning("No tags provided.");
                return;
            }

            var jsonDocument = await _zendeskApiClient.PostTags(ticketId, tags);
            _logger.LogInformation($"Response from POST tags: \n{jsonDocument.PrettifyJsonDocument()}");
        }
    }
}
