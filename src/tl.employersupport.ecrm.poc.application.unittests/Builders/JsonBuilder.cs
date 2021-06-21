﻿using System.Reflection;
using tl.employersupport.ecrm.poc.application.Extensions;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public static class JsonBuilder
    {
        private static string GetRootPath() => Assembly.GetExecutingAssembly().GetName().Name;

        public static string BuildValidTicketResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTicketWithSideloadsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_with_sideloads.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTicketCommentsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_comments.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTicketAuditsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_audits.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTagsResponse() =>
            $"{GetRootPath()}.Data.zendesk_tags.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTicketFieldsResponse =>
            $"{GetRootPath()}.Data.zendesk_ticket_fields.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTicketSearchResultsResponse =>
            $"{GetRootPath()}.Data.zendesk_ticket_search_result.json"
                .ReadManifestResourceStreamAsString();
    }
}