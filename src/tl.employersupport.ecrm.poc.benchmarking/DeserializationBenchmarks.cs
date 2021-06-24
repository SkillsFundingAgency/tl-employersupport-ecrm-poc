using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;

namespace tl.employersupport.ecrm.poc.benchmarking
{
    [MinColumn]
    [MaxColumn]
    public class DeserializationBenchmarks
    {
        private readonly string _ticketJson;
        private readonly Stream _ticketJsonStream;
        private readonly Stream _ticketFieldsJsonStream;
        
        private readonly string _ticketWithSideloadsJson;
        private readonly Stream _ticketWithSideloadsJsonStream;
        
        public DeserializationBenchmarks()
        {
            var rootPath = Assembly.GetExecutingAssembly().GetName().Name;

            _ticketJson = $"{rootPath}.Data.zendesk_ticket.json"
                .ReadManifestResourceStreamAsString();

            _ticketWithSideloadsJson = $"{rootPath}.Data.zendesk_ticket_with_sideloads.json"
                .ReadManifestResourceStreamAsString();

            _ticketJsonStream = $"{rootPath}.Data.zendesk_ticket.json"
                .GetManifestResourceStream();

            _ticketWithSideloadsJsonStream = $"{rootPath}.Data.zendesk_ticket_with_sideloads.json"
                .GetManifestResourceStream();

            _ticketFieldsJsonStream = $"{rootPath}.Data.zendesk_ticket_fields.json"
                .GetManifestResourceStream();
        }

#if !DEBUG
        [Benchmark]
#endif
        public void DeserializeTicketFromJson()
        {
            _ticketJson.DeserializeZendeskTicketResponse();
        }

#if !DEBUG
        [Benchmark]
#endif
        public void DeserializeTicketWithSideloadsFromJson()
        {
            _ticketWithSideloadsJson.DeserializeZendeskTicketResponse();
        }

#if !DEBUG
        [Benchmark]
#endif
        public async Task BuildTicketFromJsonStream()
        {
            _ticketJsonStream.Position = 0;
            var jsonDocument = await JsonDocument.ParseAsync(_ticketJsonStream);
            BuildTicket(jsonDocument);
        }
        
#if !DEBUG
        [Benchmark]
#endif
        public async Task BuildTicketFromTicketWithSideloadsJsonStream()
        {
            _ticketWithSideloadsJsonStream.Position = 0;
            var jsonDocument = await JsonDocument.ParseAsync(_ticketWithSideloadsJsonStream);
            BuildTicket(jsonDocument);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private EmployerContactTicket BuildTicket(JsonDocument jsonDocument)
        {
            if (jsonDocument != null)
            {
                var ticketElement =
                    jsonDocument.RootElement
                        .GetProperty("ticket");

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

                var ticket = new EmployerContactTicket
                {
                    Id = ticketElement.GetProperty("id").GetInt64(),
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt,
                    Tags = tags
                };

                return ticket;
            }

            return null;
        }

#if !DEBUG
        [Benchmark]
#endif
        public async Task<IDictionary<long, TicketField>> BuildTicketFieldsFromJsonStream()
        {
            _ticketFieldsJsonStream.Position = 0;
            var jsonDocument = await JsonDocument.ParseAsync(_ticketFieldsJsonStream);

            var dictionary = jsonDocument.RootElement
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

#if !DEBUG
        [Benchmark]
#endif
        public async Task<IDictionary<long, TicketField>> BuildTLevelTicketFieldsFromJsonStream()
        {
            _ticketFieldsJsonStream.Position = 0;
            var jsonDocument = await JsonDocument.ParseAsync(_ticketFieldsJsonStream);

            var dictionary = jsonDocument.RootElement
                .GetProperty("ticket_fields")
                .EnumerateArray()
                .Where(x => x.SafeGetString("title").StartsWith("T Level"))
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


#if !DEBUG
        [Benchmark]
#endif
        public async Task<IDictionary<long, TicketField>> BuildTLevelTicketFieldsFromJsonStreamWithoutSafe()
        {
            _ticketFieldsJsonStream.Position = 0;
            var jsonDocument = await JsonDocument.ParseAsync(_ticketFieldsJsonStream);

            var dictionary = jsonDocument.RootElement
                .GetProperty("ticket_fields")
                .EnumerateArray()
                .Where(x => x.GetProperty("title").GetString()!.StartsWith("T Level"))
                .Select(x =>
                    new TicketField
                    {
                        Id = x.GetProperty("id").GetInt64(),
                        Title = x.GetProperty("title").GetString(),
                        Type = x.GetProperty("type").GetString(),
                        Active = x.GetProperty("active").GetBoolean() ||
                                 x.GetProperty("collapsed_for_agents").GetBoolean()
                    })
                .ToDictionary(t => t.Id,
                    t => t);

            return dictionary;
        }
    }
}
