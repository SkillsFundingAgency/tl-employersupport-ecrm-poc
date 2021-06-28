using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class JsonDocumentDeserializationExtensions
    {
        public static TicketFieldCollection DeserializeTicketFields(this JsonDocument jsonDocument)
        {
            return new(
                jsonDocument
                .RootElement
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
                    t => t)
                );
        }

        public static SafeTags ExtractTicketSafeTags(this JsonDocument jsonDocument)
        {
            var ticketElement =
                jsonDocument.RootElement
                    .GetProperty("ticket");

            var updatedAtString = ticketElement.SafeGetString("updated_at");
            if (!DateTimeOffset.TryParse(updatedAtString, out var updatedAt))
            {
                //    _logger.LogWarning($"Could not read updated-at date for ticket {ticketId}.");
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

        public static IList<OrganisationDetail> DeserializeOrganisationDetails(this JsonDocument jsonDocument)
        {
            return jsonDocument
                .RootElement
                .TryGetProperty("organizations", out var organisationElement)
                ? organisationElement
                    .EnumerateArray()
                    .Select(o => new OrganisationDetail
                    {
                        Id = o.GetProperty("id").GetInt64(),
                        Name = o.GetProperty("name").GetString()
                    })
                    .ToList()
                : new List<OrganisationDetail>();
        }

        public static IList<UserDetail> DeserializeUserDetails(this JsonDocument jsonDocument)
        {
            return jsonDocument
                .RootElement
                .TryGetProperty("users", out var userElement)
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
        }

        public static EmployerContactTicket ToEmployerContactTicket(this JsonDocument jsonDocument, TicketFieldCollection fieldDefinitions)
        {
            if (jsonDocument != null)
            {
                var ticketElement = jsonDocument.RootElement.GetProperty("ticket");

                var createdAtString = ticketElement.SafeGetString("created_at");
                if (!DateTimeOffset.TryParse(createdAtString, out var createdAt))
                {
                    //_logger.LogWarning($"Could not read created_at date for ticket {ticketId}.");
                }

                var updatedAtString = ticketElement.SafeGetString("updated_at");
                if (!DateTimeOffset.TryParse(updatedAtString, out var updatedAt))
                {
                    //_logger.LogWarning($"Could not read updated-at date for ticket {ticketId}.");
                }

                var tags = ticketElement
                    .GetProperty("tags")
                    .EnumerateArray()
                    .Select(t => t.SafeGetString())
                    .ToList();

                var users = jsonDocument.DeserializeUserDetails();

                var requesterId = ticketElement.SafeGetInt64("requester_id");
                var user = users.FirstOrDefault(o => o.Id == requesterId)
                           ?? new UserDetail();

                var organisations = jsonDocument.DeserializeOrganisationDetails();

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
            return new();
        }

        public static IList<TicketSearchResult> ToTicketSearchResultList(this JsonDocument jsonDocument)
        {
            return jsonDocument
                    .RootElement
                    .TryGetProperty("results", out var searchResultsElement)
                    ? searchResultsElement
                        .EnumerateArray()
                        .Select(x => new TicketSearchResult
                        {
                            Id = x.SafeGetInt64("id"),
                            Subject = x.SafeGetString("subject"),
                        })
                        .OrderBy(x => x.Id)
                        .ToList()
                    : new List<TicketSearchResult>();
        }
    }
}
