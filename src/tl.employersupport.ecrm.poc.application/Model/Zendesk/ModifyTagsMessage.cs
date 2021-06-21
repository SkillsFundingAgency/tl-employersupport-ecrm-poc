using System.Collections.Generic;

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class ModifyTagsMessage
    {
        public long TicketId { get; init; }

        public IList<string> Tags { get; init; }
    }
}
