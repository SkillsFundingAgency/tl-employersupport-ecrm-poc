// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class User
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonPropertyName("time_zone")]
        public string TimeZone { get; set; }
        [JsonPropertyName("iana_time_zone")]
        public string IanaTimeZone { get; set; }
        public object Phone { get; set; }
        [JsonPropertyName("shared_phone_number")]
        public object SharedPhoneNumber { get; set; }
        public object Photo { get; set; }
        [JsonPropertyName("locale_id")]
        public int LocaleId { get; set; }
        public string Locale { get; set; }
        [JsonPropertyName("organization_id")]
        public long OrganizationId { get; set; }
        public string Role { get; set; }
        public bool Verified { get; set; }
        [JsonPropertyName("external_id")]
        public object ExternalId { get; set; }
        public object[] Tags { get; set; }
        public string Alias { get; set; }
        public bool Active { get; set; }
        public bool Shared { get; set; }
        [JsonPropertyName("shared_agent")]
        public bool SharedAgent { get; set; }
        [JsonPropertyName("last_login_at")]
        public DateTime LastLoginAt { get; set; }
        [JsonPropertyName("two_factor_auth_enabled")]
        public object TwoFactorAuthEnabled { get; set; }
        public string Signature { get; set; }
        public string Details { get; set; }
        public string Notes { get; set; }
        [JsonPropertyName("Role_type")]
        public object RoleType { get; set; }
        [JsonPropertyName("custom_role_id")]
        public object CustomRoleId { get; set; }
        public bool   Moderator { get; set; }
        [JsonPropertyName("ticket_restriction")]
        public object TicketRestriction { get; set; }
        [JsonPropertyName("only_private_comments")]
        public bool? OnlyPrivateComments { get; set; }
        [JsonPropertyName("restricted_agent")]
        public bool? RestrictedAgent { get; set; }
        public bool? Suspended { get; set; }
        [JsonPropertyName("chat_only")]
        public bool? ChatOnly { get; set; }
        [JsonPropertyName("default_group_id")]
        public long DefaultGroupId { get; set; }
        [JsonPropertyName("report_csv")]
        public bool? ReportCsv { get; set; }
        [JsonPropertyName("user_fields")]
        public UserFields UserFields { get; set; }
    }
}