﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //Note: check this last otherwise unit tests checking null ctor args will fail
            if (zendeskConfiguration is null)
                throw new ArgumentNullException(nameof(zendeskConfiguration));

            _zendeskConfiguration = zendeskConfiguration.Value ??
                                    throw new ArgumentNullException(
                                        $"{nameof(zendeskConfiguration)}.{nameof(zendeskConfiguration.Value)}",
                                        "Configuration value must not be null");
        }

        public async Task<EmployerContactTicket> GetEmployerContactTicket(long ticketId)
        {
            _logger.LogInformation($"Getting ticket {ticketId}");

            var jsonDoc = await _zendeskApiClient.GetTicketJsonDocument(ticketId, Sideloads.GetTicketSideloads());

            if (jsonDoc != null)
            {
                var ticketElement = jsonDoc.RootElement.GetProperty("ticket");

                var createdAtString = ticketElement.SafeGetString("created_at");
                if (!DateTimeOffset.TryParse(createdAtString, out var createdAt))
                {
                    _logger.LogWarning($"Could not read created_at date for ticket {ticketId}.");
                }

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

                var users = jsonDoc.RootElement.TryGetProperty("users", out var userElement)
                    ? userElement
                        .EnumerateArray()
                        .Select(u => new UserDetail
                        {
                            Id = u.GetProperty("id").GetInt64(),
                            Name = u.GetProperty("name").GetString(),
                            Email = u.GetProperty("email").GetString()
                        })
                        .ToList()
                    : new List<UserDetail>();

                var requesterId = ticketElement.SafeGetInt64("requester_id");
                var user = users.FirstOrDefault(o => o.Id == requesterId)
                           ?? new UserDetail();

                var organisations = jsonDoc.RootElement.TryGetProperty("organizations", out var organisationElement)
                    ? organisationElement
                        .EnumerateArray()
                        .Select(o => new OrganisationDetail
                        {
                            Id = o.GetProperty("id").GetInt64(),
                            Name = o.GetProperty("name").GetString()
                        })
                        .ToList()
                    : new List<OrganisationDetail>();

                var organisationId = ticketElement.SafeGetInt64("organization_id");
                var organisation = organisations.FirstOrDefault(o => o.Id == organisationId)
                                   ?? new OrganisationDetail();

                var ticket = new EmployerContactTicket
                {
                    Id = ticketElement.GetProperty("id").GetInt64(),
                    Description = ticketElement.SafeGetString("description"),
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt,
                    Tags = tags,
                    Organisation = organisation,
                    RequestedBy = user
                };

                //Build fields
                var fieldDefinitions = await GetTicketFields();
                //TODO: Just pull out the ones we care about, to reduce lookups
                var tLevelFields = fieldDefinitions
                    .Where(d =>
                        d.Value.Title.StartsWith("T Level"))
                    .ToDictionary(d => d.Key,
                        t => t.Value);

                foreach (var f in tLevelFields)
                {
                    Debug.WriteLine($"{f.Value.Title} - {f.Value.Type}");
                }

                Debug.WriteLine("Looking at standard fields");
                if (ticketElement.TryGetProperty("custom_fields", out var customFieldsElement))
                {
                    foreach (var fieldElement in customFieldsElement.EnumerateArray())
                    {
                        var fieldId = fieldElement.GetProperty("id").GetInt64();
                        if (tLevelFields.TryGetValue(fieldId, out var definition))
                        {
                            if (definition.Title.StartsWith("T Level"))
                            {
                                Debug.WriteLine($"{definition.Title} {definition.Type}");
                                var fieldValue = fieldElement.SafeGetString("value");
                                Debug.WriteLine($"    '{fieldValue}' ({fieldValue is null})");

                                switch (definition.Title)
                                {
                                    case "T Levels Employers - Contact Method":
                                        ticket.ContactMethod = fieldValue;
                                        break;
                                    case "T Level Name":
                                        ticket.ContactName = fieldValue;
                                        break;
                                    case "T Levels Employers - Phone Number":
                                        ticket.Phone = fieldValue;
                                        break;
                                    case "T Levels Employers - Total Employees":
                                        ticket.EmployerSize = fieldValue;
                                        break;
                                    case "T Levels Employers - Company Name":
                                        ticket.EmployerName = fieldValue;
                                        break;
                                    case "T Levels Employers - Contact reason":
                                        ticket.ContactReason = fieldValue;
                                        break;
                                    case "T Levels Query Subject":
                                        ticket.QuerySubject = fieldValue;
                                        break;
                                    default:
                                        Debug.WriteLine($"Found missing field {definition.Title}");
                                        break;
                                }
                            }
                        }
                    }
                }

                Debug.WriteLine("Looking at standard fields");
                if (ticketElement.TryGetProperty("fields", out var fieldsElement))
                {
                    foreach (var fieldElement in fieldsElement.EnumerateArray())
                    {
                        var fieldId = fieldElement.GetProperty("id").GetInt64();
                        if (tLevelFields.TryGetValue(fieldId, out var definition))
                        {
                            if (definition.Title.StartsWith("T Level"))
                            {
                                Debug.WriteLine($"{definition.Title} {definition.Type}");
                                var fieldValue = fieldElement.SafeGetString("value");
                                Debug.WriteLine($"    '{fieldValue}' ({fieldValue is null})");
                            }
                        }
                    }
                }

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

        public async Task<IDictionary<long, TicketField>> GetTicketFields()
        {
            var jsonDoc = await _zendeskApiClient.GetTicketFieldsJsonDocument();

            if (jsonDoc != null)
            {
                var count = jsonDoc.RootElement.GetProperty("count");
                var nextPage = jsonDoc.RootElement.GetProperty("next_page");
                _logger.LogInformation($"GetTicketFields found {count} items. Next page is '{nextPage}'");

                var dictionary = jsonDoc.RootElement
                    .GetProperty("ticket_fields")
                    .EnumerateArray()
                    .Select(x =>
                        new TicketField
                        {
                            Id = x.SafeGetInt64("id"),
                            Title = x.SafeGetString("title"),
                            Type = x.SafeGetString("type"),
                            Active = x.SafeGetBoolean("active") || x.SafeGetBoolean("collapsed_for_agents")
                        })
                    .ToDictionary(t => t.Id,
                        t => t);

                return dictionary;
            }

            return new Dictionary<long, TicketField>();
        }

        public async Task<SafeTags> GetTicketTags(long ticketId)
        {
            var jsonDoc = await _zendeskApiClient.GetTicketJsonDocument(ticketId);

            if (jsonDoc != null)
            {
                var ticketElement =
                    jsonDoc.RootElement
                        .GetProperty("ticket");

                //var createdAtString = ticketElement.SafeGetString("created_at");
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

            var jsonDoc = await _zendeskApiClient.GetTicketSearchResultsJsonDocument(query);

            if (jsonDoc != null)
            {
                //var temp =jsonDoc.PrettifyJsonDocument();
                //TODO: Worry about paging? Or only get recent ones?
                //Other fields:
                //ticket_form_id=360001820480
                var count = jsonDoc.RootElement.GetProperty("count");
                var nextPage = jsonDoc.RootElement.GetProperty("next_page");
                _logger.LogInformation($"Search found {count} items. Next page is '{nextPage}'");
                var searchResults = jsonDoc.RootElement
                    .GetProperty("results")
                    .EnumerateArray()
                    .Select(x => new TicketSearchResult
                    {
                        Id = x.SafeGetInt64("id"),
                        Subject = x.SafeGetString("subject"),
                    })
                    .OrderBy(x => x.Id)
                    .ToList();

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
            
            var jsonDoc = await _zendeskApiClient.PutTags(ticketId, tags);

            _logger.LogInformation($"Response from PUT tag: \n{jsonDoc.PrettifyJsonDocument()}");
        }

        public async Task ModifyTags(long ticketId, SafeTags tags)
        {
            _logger.LogInformation($"Adding tags for ticket {ticketId}");

            if (tags is null)
            {
                _logger.LogWarning("No tags provided.");
                return;
            }
            
            var jsonDoc = await _zendeskApiClient.PostTags(ticketId, tags);
            _logger.LogInformation($"Response from POST tags: \n{jsonDoc.PrettifyJsonDocument()}");
        }
    }
}
