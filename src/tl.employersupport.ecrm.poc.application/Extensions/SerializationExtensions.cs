using System;
using System.Collections.Generic;
using System.Text.Json;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using Ticket = tl.employersupport.ecrm.poc.application.Model.Zendesk.Ticket;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class SerializationExtensions
    {
        private static readonly JsonSerializerOptions SerializerOptions
            = new()
            {
                PropertyNameCaseInsensitive = true,
                //IgnoreNullValues = true,
                //TODO: Future System.Text.Json version should have snake case support
                //PropertyNamingPolicy = JsonNamingPolicy.SnakeCase
            };

        public static Ticket DeserializeZendeskTicket(this string json)
        {
            var ticketResponse = JsonSerializer
                .Deserialize<TicketResponse>(
                    json,
                    SerializerOptions);

            return ticketResponse?.Ticket;
        }

        public static TicketResponse DeserializeZendeskTicketResponse(this string json)
        {
            return JsonSerializer
                .Deserialize<TicketResponse>(
                    json,
                    SerializerOptions);
        }

        public static IEnumerable<Audit> DeserializeZendeskAudits(this string json)
        {
            var auditResponse = JsonSerializer
                .Deserialize<AuditResponse>(
                    json,
                    SerializerOptions);

            return auditResponse != null
                ? auditResponse.Audits
                : Array.Empty<Audit>();
        }

        public static IEnumerable<Comment> DeserializeZendeskComments(this string json)
        {
            var commentResponse = JsonSerializer
                .Deserialize<CommentResponse>(
                    json,
                    SerializerOptions);

            return commentResponse != null 
                ? commentResponse.Comments 
                : Array.Empty<Comment>();
        }
    }
}
