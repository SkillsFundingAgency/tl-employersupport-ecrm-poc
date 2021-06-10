using System;
using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class Ticket
    {
        public string Url { get; set; }
        public long Id { get; set; }
        [JsonPropertyName("external_id")]
        public object ExternalId { get; set; }
        public Via Via { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        [JsonPropertyName("raw_subject")]
        public string RawSubject { get; set; }
        public string Description { get; set; }
        public object Priority { get; set; }
        public string Status { get; set; }
        public object Recipient { get; set; }
        [JsonPropertyName("requester_id")]
        public long RequesterId { get; set; }
        [JsonPropertyName("submitter_id")]
        public long SubmitterId { get; set; }
        [JsonPropertyName("assignee_id")]
        public object AssigneeId { get; set; }
        [JsonPropertyName("organization_id")]
        public long OrganizationId { get; set; }
        [JsonPropertyName("group_id")]
        public object GroupId { get; set; }
        [JsonPropertyName("collaborator_ids")]
        public object[] CollaboratorIds { get; set; }
        [JsonPropertyName("follower_ids")]
        public object[] FollowerIds { get; set; }
        [JsonPropertyName("email_cc_ids")]
        public object[] EmailCcIds { get; set; }
        [JsonPropertyName("forum_topic_id")]
        public object ForumTopicId { get; set; }
        [JsonPropertyName("problem_id")]
        public object ProblemId { get; set; }
        [JsonPropertyName("has_incidents")]
        public bool? HasIncidents { get; set; }
        [JsonPropertyName("is_public")]
        public bool? IsPublic { get; set; }
        [JsonPropertyName("due_at")]
        public object DueAt { get; set; }
        public string[] Tags { get; set; }
        [JsonPropertyName("custom_fields")]
        public CustomFields[] CustomFields { get; set; }
        [JsonPropertyName("satisfaction_rating")]
        public SatisfactionRating SatisfactionRating { get; set; }
        [JsonPropertyName("sharing_agreement_ids")]
        public object[] SharingAgreementIds { get; set; }
        [JsonPropertyName("comment_count")]
        public int CommentCount { get; set; }
        public Field[] Fields { get; set; }
        [JsonPropertyName("followup_ids")]
        public object[] FollowupIds { get; set; }
        [JsonPropertyName("ticket_form_id")]
        public long TicketFormId { get; set; }
        [JsonPropertyName("brand_id")]
        public long BrandId { get; set; }
        [JsonPropertyName("satisfaction_probability")]
        public object SatisfactionProbability { get; set; }
        [JsonPropertyName("allow_channelback")]
        public bool? AllowChannelBack { get; set; }
        [JsonPropertyName("allow_attachments")]
        public bool? AllowAttachments { get; set; }
    }
}
