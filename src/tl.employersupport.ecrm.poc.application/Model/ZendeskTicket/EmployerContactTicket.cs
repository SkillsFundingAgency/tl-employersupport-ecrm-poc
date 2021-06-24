using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.ZendeskTicket
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class EmployerContactTicket
    {
        public long Id { get; init; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<string> Tags { get; set; }

        private string DebuggerDisplay()
            => $"{nameof(EmployerContactTicket)} " +
               $"{nameof(Id)}={Id} " +
               $"{nameof(CreatedAt)}={CreatedAt:yyyy-MM-dd HH:mm:ss}";
    }
}
