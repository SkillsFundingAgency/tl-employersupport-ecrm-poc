// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class Organization
    {
        public string Url { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("shared_tickets")]
        public bool SharedTickets { get; set; }
        [JsonPropertyName("shared_comments")]
        public bool SharedComments { get; set; }
        [JsonPropertyName("external_id")]
        public object ExternalId { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonPropertyName("domain_names")]
        public string[] DomainNames { get; set; }
        public string Details { get; set; }
        public string Notes { get; set; }
        [JsonPropertyName("group_id")]
        public object GroupId { get; set; }
        public string[] Tags { get; set; }
        [JsonPropertyName("organization_fields")]
        public OrganizationFields OrganizationFields { get; set; }
    }
}