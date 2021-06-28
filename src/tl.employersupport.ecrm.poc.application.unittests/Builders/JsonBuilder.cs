using System.Reflection;
using tl.employersupport.ecrm.poc.application.Extensions;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public static class JsonBuilder
    {
        private static string GetRootPath() => Assembly.GetExecutingAssembly().GetName().Name;

        public static string BuildTicketResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildTicketWithSideloadsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_with_sideloads.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildTicketCommentsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_comments.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildTicketAuditsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_audits.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildTagsResponse() =>
            $"{GetRootPath()}.Data.zendesk_tags.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildTicketFieldsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_fields.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildTicketSearchResultsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_search_result.json"
                .ReadManifestResourceStreamAsString();
    }
}
