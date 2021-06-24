using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.ZendeskTicket
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class EmployerContactTicket
    {
        public long Id { get; init; }

        public string Description { get; init; }

        public string EmployerName { get; set; }
        public string Postcode { get; init; }
        public string AddressLine1 { get; init; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string EmployerSize { get; set; }
        public string ContactMethod { get; set; }
        public string ContactReason { get; set; }
        public string QuerySubject { get; set; }


        public OrganisationDetail Organisation { get; init; }

        public UserDetail RequestedBy { get; init; }

        public List<string> Tags { get; init; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        private string DebuggerDisplay()
            => $"{nameof(EmployerContactTicket)} " +
               $"{nameof(Id)}={Id} " +
               $"{nameof(CreatedAt)}={CreatedAt:yyyy-MM-dd HH:mm:ss}";
    }
}
