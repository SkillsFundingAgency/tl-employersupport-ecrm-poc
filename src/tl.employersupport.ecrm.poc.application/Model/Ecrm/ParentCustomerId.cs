using System;
using System.Text.Json.Serialization;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    public class ParentCustomerId
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("logicalname")]
        public string LogicalName { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }
    }
}