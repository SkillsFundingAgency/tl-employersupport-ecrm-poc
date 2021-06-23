using System;
using System.Collections.Generic;

namespace tl.employersupport.ecrm.poc.application.Model
{
    public class EmployerContactTicket
    {
        public long Id { get; init; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<string> Tags { get; set; }
    }
}
