using System.Reflection;
using tl.employersupport.ecrm.poc.application.Extensions;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public static class JsonBuilder
    {
        private static string GetRootPath() => Assembly.GetExecutingAssembly().GetName().Name;

        public static string BuildEcrmAccount() =>
            $"{GetRootPath()}.Data.ecrm_account.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildEcrmAccountList() =>
            $"{GetRootPath()}.Data.ecrm_account_list.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildEcrmWhoAmIResponse() =>
            $"{GetRootPath()}.Data.ecrm_who_am_i.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildZendeskTicketResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildZendeskTicketWithSideloadsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_with_sideloads.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildZendeskTicketCommentsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_comments.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildZendeskTicketAuditsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_audits.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildZendeskTagsResponse() =>
            $"{GetRootPath()}.Data.zendesk_tags.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildZendeskTicketFieldsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_fields.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildZendeskTicketSearchResultsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_search_result.json"
                .ReadManifestResourceStreamAsString();
    }
}
