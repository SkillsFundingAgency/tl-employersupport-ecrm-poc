using System.Diagnostics.CodeAnalysis;

namespace tl.employersupport.ecrm.poc.application.Model
{
    public class Sideloads
    {
        [SuppressMessage("ReSharper", "CommentTypo")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public static string GetTicketSideloads() =>
            //TODO: Consider an enum or class to manage sideloads
            //Allowed sideloads:
            //users, groups, organizations, lastaudits, metricsets, dates, sharingagreements, commentcount, incidentcounts, ticketforms, metric_events(single ticket), slas(single ticket)
            "comment_count,users,groups,commentcount,ticketforms,organizations,audits";
    }
}
