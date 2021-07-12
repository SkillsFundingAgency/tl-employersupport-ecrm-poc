using System;
using System.Collections.Generic;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmAccountBuilder
    {
        public Account Build() =>
            Build(Guid.Parse("3fe7a4a8-83a0-49cc-872f-927d71654b86"));

        public Account Build(Guid? accountId) =>
            new()
            {
                AccountId = accountId,
                Name = "Test Account"
            };

        public IList<Account> BuildList() =>
            new List<Account>()
            {
                new()
                {
                    AccountId = Guid.Parse("56b4a1e5-a9e3-4bae-9690-09b4b009d482"),
                    Name = "Test Account 1"
                },
                new()
                {
                    AccountId = Guid.Parse("f59bd75a-4dd9-4cca-8778-124085752ba6"),
                    Name = "Test Account 2"
                },
                new()
                {
                    AccountId = Guid.Parse("a50d5328-8102-46ae-b2fd-0e6ddc1a472a"),
                    Name = "Test Account 3"
                }
            };
    }
}
