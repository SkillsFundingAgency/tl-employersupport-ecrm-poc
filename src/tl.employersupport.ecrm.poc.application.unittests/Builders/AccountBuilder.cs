using System;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class AccountBuilder
    {
        public Account Build() => 
            Build(Guid.Parse("80B7D3DC-D54E-4277-8B25-D8891ADA4DC4"));

        public Account Build(Guid accountId) =>
            new()
            {
                AccountId = accountId,
                Name = "Test Account"
            };

    }
}
