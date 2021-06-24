using System.Collections.Generic;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.Model.ZendeskTicket
{
    public class CombinedTicket
    {
        public long Id { get; init; }
        
        public Ticket Ticket { get; init; }

        public IEnumerable<Audit> Audits { get; init; }
        public IEnumerable<Comment> Comments { get; init; }
        public IEnumerable<Group> Groups { get; init; }
        public IEnumerable<User> Users { get; init; }
        public IEnumerable<Organization> Organizations { get; init; }
    }
}
