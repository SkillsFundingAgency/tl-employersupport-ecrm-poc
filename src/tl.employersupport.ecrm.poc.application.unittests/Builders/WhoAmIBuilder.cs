using System;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class WhoAmIBuilder
    {
        public WhoAmI Build() => 
            new()
            {
                BusinessUnitId = Guid.Parse("eed747ad-6aba-4af9-b92a-9843e506c28e"),
                UserId = Guid.Parse("92f7f5e5-dfb6-4833-8b21-da22e1ecdb1a"),
                OrganizationId = Guid.Parse("3c8902ef-83cc-4d22-a083-0d0f1e8fcf83")
            };
    }
}
