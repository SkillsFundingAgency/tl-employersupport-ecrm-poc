using System;
using System.Collections.Generic;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;

namespace tl.employersupport.ecrm.poc.application.functions.unittests.Builders
{
    public class EmployerContactTicketBuilder
    {
        private EmployerContactTicket _employerContactTicket;

        public EmployerContactTicket Build() =>
            _employerContactTicket;

        public EmployerContactTicketBuilder WithDefaultValues()
        {
            _employerContactTicket = new EmployerContactTicket
            {
                Id = 4485,
                Organisation = new OrganisationDetail
                {
                    Id = 456876,
                    Name = "Large Corporation Limited"
                },
                RequestedBy = new UserDetail
                {
                    Id = 12855186,
                    Name = "Bob Bobs",
                    Email = "bob.bobs@bobbobs.com"
                },
                CreatedAt = DateTimeOffset.Parse("2021-06-23 14:40"),
                UpdatedAt = DateTimeOffset.Parse("2021-06-24 12:19"),
                Tags = new List<string>
                {
                     "100-1000__large_",
                     "customer",
                     "tlevels-email",
                     "tlevels_approved"
                 }
            };
            return this;
        }
    }
}
