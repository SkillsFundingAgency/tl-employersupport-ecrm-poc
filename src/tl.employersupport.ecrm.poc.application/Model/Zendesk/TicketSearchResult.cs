// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    [DebuggerDisplay("TicketSearchResult {" + nameof(Id) + "} " +
                     "{" + nameof(Subject) + "}")]
    public class TicketSearchResult
    {
        public long Id { get; init; }
        public string Subject { get; init; }
    }
}
