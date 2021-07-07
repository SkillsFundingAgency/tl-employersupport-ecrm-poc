using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class Account
    {
        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("accountid")]
        public Guid AccountId { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        private string DebuggerDisplay()
            => $"{AccountId} " +
               $"{Name} ";
    }
}
