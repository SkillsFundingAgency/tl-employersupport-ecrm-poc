using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class Note
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; init; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("accountid")]
        public Guid? AccountId { get; init; }

        //entity.filename = "File.txt";
        //entity.isdocument = true;
        //entity.documentbody = "TU0gVGV4dCBGaWxl";

        [JsonIgnore]
        public Guid ParentAccountId { get; init; }

        // ReSharper disable once StringLiteralTypo
        //[JsonPropertyName("parentcustomerid")]
        [JsonPropertyName("subject")]
        public string Subject { get; init; }
        [JsonPropertyName("note_text")]
        public string NoteText { get; init; }
        [JsonPropertyName("filename")]
        public string Filename { get; init; }
        [JsonPropertyName("is_document")]
        public bool IsDocument { get; init; }

        [JsonPropertyName("document_body")]
        public string DocumentBody { get; init; }

        private string DebuggerDisplay()
            => //$"{AccountId} " +
               $"{Subject}";
    }
}
