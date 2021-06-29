using System;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.functions.unittests.Builders
{
    public class EmployerBuilder
    {
        public Employer Build() =>
            new()
            {
                AccountId = Guid.Parse("461082b5-d2ea-475b-bf85-2417e650aa68"),
                CompanyName = "Fake Company"
            };
    }
}
