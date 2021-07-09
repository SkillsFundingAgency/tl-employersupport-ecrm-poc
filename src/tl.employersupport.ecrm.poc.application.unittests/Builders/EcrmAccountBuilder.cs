using System;
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

    }
}
