
using System;
using System.Collections.Generic;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.Model
{
    public class CombinedTicket
    {
        public long Id { get; init; }
        
        public Ticket Ticket { get; set; }

        public IEnumerable<Audit> Audits { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Organization> Organizations { get; set; }
    }
}
