using System;
using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class Audit
    {
        public long Id { get; set; }
        [JsonPropertyName("ticket_id")]
        public int TicketId { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("author_id")]
        public long AuthorId { get; set; }
        public Event[] Events { get; set; }
        public Via Via { get; set; }
    }
}
