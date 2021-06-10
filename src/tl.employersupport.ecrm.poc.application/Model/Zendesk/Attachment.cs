using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class Attachment
    {
        public string Url { get; set; }
        public long Id { get; set; }
        [JsonPropertyName("file_name")]
        public string FileName { get; set; }
        [JsonPropertyName("content_url")]
        public string ContentUrl { get; set; }
        [JsonPropertyName("mapped_content_url")]
        public string MappedContentUrl { get; set; }
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }
        public int Size { get; set; }
        public object Width { get; set; }
        public object Height { get; set; }
        public bool Inline { get; set; }
        public bool Deleted { get; set; }
        public object[] Thumbnails { get; set; }
    }
}