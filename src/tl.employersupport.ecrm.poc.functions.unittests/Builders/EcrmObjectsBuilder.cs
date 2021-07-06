using System;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.functions.unittests.Builders
{
    public class EcrmObjectsBuilder
    {
        public WhoAmIResponse BuildWhoAmIResponse() =>
            new()
            {
                BusinessUnitId = Guid.Parse("cc4d685a-cb20-40a6-99b9-656e793addeb"),
                UserId = Guid.Parse("82b241df-982b-49f7-b3ad-b7d060dde75d"),
                OrganizationId = Guid.Parse("0eae3056-9360-4eb0-b728-d890257c63be")
            };
    }
}
