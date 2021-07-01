
namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Sideloads
    {
        // ReSharper disable CommentTypo
        // ReSharper disable StringLiteralTypo
        public static string GetTicketSideloads() =>
            //TODO: Consider an enum or class to manage sideloads
            //Allowed sideloads:
            //users, groups, organizations, lastaudits, metricsets, dates, sharingagreements, commentcount, incidentcounts, ticketforms, metric_events(single ticket), slas(single ticket)
            "users,groups,organizations,audits,metric_events";
        // ReSharper restore CommentTypo
    }
}
