using System;
using System.Collections.Generic;
using System.Text.Json;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using Ticket = tl.employersupport.ecrm.poc.application.Model.Zendesk.Ticket;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class SerializationExtensions
    {
        public static Ticket DeserializeZendeskTicket(this string json)
        {
            var ticketResponse = JsonSerializer
                .Deserialize<TicketResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);

            return ticketResponse?.Ticket;
        }

        public static TicketResponse DeserializeZendeskTicketResponse(this string json)
        {
            return JsonSerializer
                .Deserialize<TicketResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);
        }

        public static IEnumerable<Audit> DeserializeZendeskAudits(this string json)
        {
            var auditResponse = JsonSerializer
                .Deserialize<AuditResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);

            return auditResponse != null
                ? auditResponse.Audits
                : Array.Empty<Audit>();
        }

        public static IEnumerable<Comment> DeserializeZendeskComments(this string json)
        {
            var commentResponse = JsonSerializer
                .Deserialize<CommentResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);

            return commentResponse != null 
                ? commentResponse.Comments 
                : Array.Empty<Comment>();
        }
    }
}
