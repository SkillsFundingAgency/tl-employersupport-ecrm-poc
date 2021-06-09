using System.Reflection;
using tl.employersupport.ecrm.poc.application.Extensions;

namespace tl.employersupport.ecrm.poc.application.tests.Builders
{
    public static class JsonBuilder
    {
        private static string GetRootPath() => Assembly.GetExecutingAssembly().GetName().Name;

        public static string BuildValidTicketWithSideloadsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_with_sideloads.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTicketCommentsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_comments.json"
                .ReadManifestResourceStreamAsString();

        public static string BuildValidTicketAuditsResponse() =>
            $"{GetRootPath()}.Data.zendesk_ticket_audits.json"
                .ReadManifestResourceStreamAsString();
    }
}
