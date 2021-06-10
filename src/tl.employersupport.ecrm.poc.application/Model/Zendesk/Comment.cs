﻿using System;
using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class Comment
    {
        public long Id { get; set; }
        public string Type { get; set; }
        [JsonPropertyName("author_id")]
        public long AuthorId { get; set; }
        public string Body { get; set; }
        [JsonPropertyName("html_body")]
        public string HtmlBody { get; set; }
        [JsonPropertyName("plain_body")]
        public string PlainBody { get; set; }
        //[JsonPropertyName("public")]
        public bool Public { get; set; }
        public Attachment[] Attachments { get; set; }
        [JsonPropertyName("audit_id")]
        public long AuditId { get; set; }
        public Via Via { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}