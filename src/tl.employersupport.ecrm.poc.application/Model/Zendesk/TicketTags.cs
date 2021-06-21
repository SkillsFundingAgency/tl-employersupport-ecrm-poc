using System;
using System.Collections.Generic;

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class TicketTags
    {
        public long TicketId { get; init; }

        public IList<string> Tags { get; init; }

        public DateTimeOffset UpdatedStamp { get; init; }
    }
}
