﻿using System.IO;
using System.Text;
using System.Text.Json;
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class JsonExtensions
    {
        public static string PrettifyJsonString(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return string.Empty;
            }

            var jsonDocument = JsonDocument.Parse(json);

            return jsonDocument.PrettifyJsonDocument();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string PrettifyJsonDocument(this JsonDocument jsonDocument)
        {
            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, options))
            {
                jsonDocument.WriteTo(writer);
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static int SafeGetInt32(this JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind == JsonValueKind.Number
                   && property.TryGetInt32(out var val)
                ? val
                : default;
        }

        public static long SafeGetInt64(this JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind == JsonValueKind.Number
                   && property.TryGetInt64(out var val)
                ? val
                : default;
        }

        public static double SafeGetDouble(this JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind == JsonValueKind.Number
                   && property.TryGetDouble(out var val)
                ? val
                : default;
        }

        public static string SafeGetString(this JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind == JsonValueKind.String
                ? property.GetString()
                : default;
        }
    }
}
