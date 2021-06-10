using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class UserFields
    {
        [JsonPropertyName("address_line_1")]
        public object AddressLine1 { get; set; }
        [JsonPropertyName("address_line_2")]
        public object AddressLine2 { get; set; }
        [JsonPropertyName("address_line_3")]
        public object AddressLine3 { get; set; }
        public object City { get; set; }
        [JsonPropertyName("communities_user")]
        public object CommunitiesUser { get; set; }
        [JsonPropertyName("contact_type")]
        public object ContactType { get; set; }
        public object Postcode { get; set; }
    }
}