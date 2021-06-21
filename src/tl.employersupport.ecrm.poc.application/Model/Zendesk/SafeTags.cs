using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class SafeTags
    {
        public IList<string> Tags { get; init; }

        [JsonPropertyName("updated_stamp")]
        public DateTimeOffset UpdatedStamp { get; init; }

        [JsonPropertyName("safe_update")]
        public bool SafeUpdate { get; init; } = true;
    }
}
