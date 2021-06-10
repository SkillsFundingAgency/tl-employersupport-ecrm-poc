using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class OrganizationFields
    {
        [JsonPropertyName("account_manager")]
        public object AccountManager { get; set; }
        [JsonPropertyName("account_manager_e_mail")]
        public object AccountManagerEmail { get; set; }
        [JsonPropertyName("account_manager_status")]
        public object AccountManagerStatus { get; set; }
        [JsonPropertyName("address_line_1")]
        public object AddressLine1 { get; set; }
        [JsonPropertyName("address_line_2")]
        public object AddressLine2 { get; set; }
        [JsonPropertyName("address_line_3")]
        public object AddressLine3 { get; set; }
        public object City { get; set; }
        public object County { get; set; }
        [JsonPropertyName("main_phone")]
        public object MainPhone { get; set; }
        [JsonPropertyName("organisation_status")]
        public object OrganisationStatus { get; set; }
        [JsonPropertyName("organisation_type")]
        public object OrganisationType { get; set; }
        public object Postcode { get; set; }
    }
}