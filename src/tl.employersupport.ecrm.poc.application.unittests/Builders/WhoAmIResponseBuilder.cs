using System;
using Microsoft.Crm.Sdk.Messages;
using NSubstitute;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class WhoAmIResponseBuilder
    {
        public WhoAmIResponse Build()
        {
            var whoAmIResponse = Substitute.For<WhoAmIResponse>();
            whoAmIResponse.BusinessUnitId.Returns(Guid.Parse("92f7f5e5-dfb6-4833-8b21-da22e1ecdb1a"));
            whoAmIResponse.OrganizationId.Returns(Guid.Parse("92f7f5e5-dfb6-4833-8b21-da22e1ecdb1a"));
            whoAmIResponse.UserId.Returns(Guid.Parse("92f7f5e5-dfb6-4833-8b21-da22e1ecdb1a"));
            return whoAmIResponse;
        }

    }
}
